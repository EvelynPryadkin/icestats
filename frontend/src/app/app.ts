import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

// Interfaces - TypeScript types that define what our data looks like
// This is crucial for type safety and helps IDEs give you better autocomplete

interface Skater {
  playerId: number;
  fullName: string;
  teamName: string;
  position: string;
  goals: number;
  assists: number;
  points: number;
}

interface GoalLeader {
  playerId: number;
  fullName: string;
  position: string | null;
  teamName: string;
  gamesPlayed: number;
  goals: number;
  assists: number;
  points: number;
}

interface SyncResponse {
  message: string;
}

/**
 * Main App Component
 * 
 * This is the root component of your Angular application.
 * Think of it as the main container that holds everything else.
 */
@Component({
  selector: 'app-root',
  imports: [CommonModule, FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  // Current year for footer (computed at component creation)
  currentYear: number = new Date().getFullYear();

  // Inject HttpClient service using Angular's dependency injection system
  private http = inject(HttpClient);

  // State variables using Angular signals for reactive data binding
  skaters = signal<Skater[]>([]);
  goalLeaders = signal<GoalLeader[]>([]);
  isLoading = false;

  // Navigation tabs state - tracks which tab is currently active
  activeTab = signal<'skaters' | 'goalleaders' | 'allplayers'>('skaters');

  // Search query state - for filtering players by name or team
  searchQuery = signal('');

  // Fetch NHL data on component initialization
  ngOnInit() {
    this.loadSkaters();
    this.loadGoalLeaders();
  }

  /**
   * loadSkaters(): Fetches top 50 skaters from the API
   */
  loadSkaters() {
    this.http.get<Skater[]>('http://localhost:5048/api/nhl/leaders/skaters?limit=50')
      .subscribe({
        next: (data) => {
          console.log('First skater object:', data[0]);
          this.skaters.set(data);
        },
        error: (err) => console.error('Error loading skaters:', err)
      });
  }

  /**
   * loadGoalLeaders(): Fetches top 20 goal scorers from the API
   */
  loadGoalLeaders() {
    this.http.get<GoalLeader[]>('http://localhost:5048/api/nhl/leaders/goals?limit=20')
      .subscribe({
        next: (data) => this.goalLeaders.set(data),
        error: (err) => console.error('Error loading goal leaders:', err)
      });
  }

  /**
   * syncData(): Triggers data refresh from NHL API
   */
  syncData() {
    this.isLoading = true;
    
    const syncSkaters = this.http.post<SyncResponse>('http://localhost:5048/api/sync/skaters', {})
      .toPromise();
      
    const syncGoals = this.http.post<SyncResponse>('http://localhost:5048/api/sync/goalleaders', {})
      .toPromise();

    Promise.all([syncSkaters, syncGoals])
      .then(() => {
        this.loadSkaters();
        this.loadGoalLeaders();
      })
      .catch((err) => console.error('Sync failed:', err))
      .finally(() => {
        this.isLoading = false;
      });
  }

  /**
   * topScorer(): Finds the player with the most points
   */
  topScorer() {
    const allPlayers = [...this.skaters(), ...this.goalLeaders()];
    if (allPlayers.length === 0) return null;
    return allPlayers.sort((a, b) => b.points - a.points)[0];
  }

  /**
   * topGoalScorer(): Finds the player with the most goals
   */
  topGoalScorer() {
    const allPlayers = [...this.skaters(), ...this.goalLeaders()];
    if (allPlayers.length === 0) return null;
    return allPlayers.sort((a, b) => b.goals - a.goals)[0];
  }

  /**
   * getAllPlayers(): Combines skaters and goal leaders, removing duplicates by playerId
   */
  getAllPlayers() {
    const skaterMap = new Map<number, Skater | GoalLeader>();
    
    // Add all skaters first
    this.skaters().forEach(player => skaterMap.set(player.playerId, player));
    
    // Add goal leaders (duplicates will be overwritten since same playerId)
    this.goalLeaders().forEach(player => skaterMap.set(player.playerId, player));
    
    // Convert back to array and sort by goals descending
    return Array.from(skaterMap.values()).sort((a, b) => b.goals - a.goals);
  }

  /**
   * getFilteredPlayers(): Returns players based on active tab with search filter applied
   */
  getFilteredPlayers() {
    const query = this.searchQuery().toLowerCase();
    
    let allPlayers: (Skater | GoalLeader)[];
    
    if (this.activeTab() === 'skaters') {
      allPlayers = this.skaters();
    } else if (this.activeTab() === 'goalleaders') {
      allPlayers = this.goalLeaders();
    } else {
      // All players tab
      allPlayers = this.getAllPlayers();
    }
    
    // If search query is empty, return all filtered players
    if (!query) {
      return allPlayers;
    }
    
    // Filter by player name or team (case insensitive)
    return allPlayers.filter(player => 
      player.fullName.toLowerCase().includes(query) || 
      player.teamName.toLowerCase().includes(query)
    );
  }

  /**
   * getAbbreviation(): Converts full NHL team names to 3-letter abbreviations
   * Falls back to first 3 letters of first word if not found in map
   */
  getAbbreviation(teamName: string): string {
    const teamMap: Record<string, string> = {
      // Original teams (1924-1967)
      'Boston Bruins': 'BOS',
      'Chicago Blackhawks': 'CHI',
      'Detroit Red Wings': 'DET',
      'Montreal Canadiens': 'MTL',
      'New York Rangers': 'NYR',
      'Toronto Maple Leafs': 'TOR',
      
      // 1967 Expansion
      'Los Angeles Kings': 'LAK',
      'Philadelphia Flyers': 'PHI',
      'Pittsburgh Penguins': 'PIT',
      'St. Louis Blues': 'STL',
      
      // 1970s expansions
      'Buffalo Sabres': 'BUF',
      'Edmonton Oilers': 'EDM',
      'Vancouver Canucks': 'VAN',
      
      // Current NHL teams (1980s-2024)
      'Anaheim Ducks': 'ANA',
      'Arizona Coyotes': 'ARI',
      'Carolina Hurricanes': 'CAR',
      'Columbus Blue Jackets': 'CBJ',
      'Colorado Avalanche': 'COL',
      'Dallas Stars': 'DAL',
      'Florida Panthers': 'FLA',
      'Minnesota Wild': 'MIN',
      'Nashville Predators': 'NSH',
      'New Jersey Devils': 'NJD',
      'New York Islanders': 'NYI',
      'Ottawa Senators': 'OTT',
      'San Jose Sharks': 'SJS',
      'Seattle Kraken': 'SEA',
      'Tampa Bay Lightning': 'TBL',
      'Utah Hockey Club': 'UTA',
      'Vegas Golden Knights': 'VGK',
      'Washington Capitals': 'WSH',
      'Winnipeg Jets': 'WPG',
    };
    
    // Check exact match first
    if (teamMap[teamName]) {
      return teamMap[teamName];
    }
    
    // Fallback: take first 3 letters of the first word and uppercase them
    const parts = teamName.split(' ');
    if (parts.length > 0) {
      return parts[0].substring(0, 3).toUpperCase();
    }
    
    return teamName;
  }

   /**
    * getTeamAbbrev(): Returns team name/abbreviation for display
    */
   getTeamAbbrev(player: Skater | GoalLeader): string {
     // For skaters, convert full team name to abbreviation
     if ('teamName' in player && typeof (player as Skater).teamName === 'string') {
       return this.getAbbreviation((player as Skater).teamName);
     }
     // For goal leaders, use their team name directly (already an abbreviation)
     return (player as GoalLeader).teamName || 'Unknown';
   }

  /**
   * getPositionLabel(): Returns position label
   */
  getPositionLabel(player: Skater | GoalLeader): string {
    if ('positionCode' in player) {
      const pos = (player as GoalLeader & { positionCode?: string }).positionCode;
      return pos ? pos.charAt(0).toUpperCase() : 'N/A';
    }
    if ('position' in player) {
      const pos = player.position;
      return typeof pos === 'string' && pos.length > 0 ? pos.charAt(0).toUpperCase() : (pos || 'N/A');
    }
    return 'N/A';
  }

  /**
   * formatName(): Formats player name consistently
   */
  formatName(player: Skater | GoalLeader): string {
    if ('skaterFullName' in player) {
      return (player as GoalLeader & { skaterFullName: string }).skaterFullName;
    }
    return 'fullName' in player ? player.fullName : '';
  }

  /**
   * topScorerName(): Returns formatted name of top scorer
   */
  topScorerName() {
    const scorer = this.topScorer();
    if (!scorer) return '';
    
    return 'skaterFullName' in scorer ? scorer.skaterFullName : scorer.fullName;
  }

  /**
   * topGoalScorerName(): Returns formatted name of top goal scorer
   */
  topGoalScorerName() {
    const scorer = this.topGoalScorer();
    if (!scorer) return '';
    
    return 'skaterFullName' in scorer ? scorer.skaterFullName : scorer.fullName;
  }
}