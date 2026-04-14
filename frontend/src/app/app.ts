import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

interface Skater {
  playerId: number;
  fullName: string;
  teamName: string;
  position: string;
  gamesPlayed: number;
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

@Component({
  selector: 'app-root',
  imports: [CommonModule, FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  currentYear: number = new Date().getFullYear();
  private http = inject(HttpClient);

   skaters = signal<Skater[]>([]);
   goalLeaders = signal<GoalLeader[]>([]);
   isLoading = false;
   activeTab = signal<'skaters' | 'goalleaders' | 'allplayers'>('skaters');
   searchQuery = signal('');

  // All Players tab sorting signals
  allPlayersSortColumn = signal('points');
  allPlayersSortDirection = signal<'asc' | 'desc'>('desc');

   ngOnInit() {
     this.loadSkaters();
     this.loadGoalLeaders();
   }

  loadSkaters() {
    this.http.get<Skater[]>('http://localhost:5048/api/nhl/leaders/skaters?limit=50')
      .subscribe({
        next: (data) => { console.log(data[0]); this.skaters.set(data); },
        error: (err) => console.error(err)
      });
  }

  loadGoalLeaders() {
    this.http.get<GoalLeader[]>('http://localhost:5048/api/nhl/leaders/goals?limit=20')
      .subscribe({ next: (data) => this.goalLeaders.set(data), error: (err) => console.error(err) });
  }

  syncData() {
      this.isLoading = true;
      const syncSkaters = this.http.post<SyncResponse>('http://localhost:5048/api/sync/skaters', {}).toPromise();
      const syncGoals = this.http.post<SyncResponse>('http://localhost:5048/api/sync/goalleaders', {}).toPromise();
      Promise.all([syncSkaters, syncGoals]).then(() => {
        this.loadSkaters();
        this.loadGoalLeaders();
      }).catch((err) => console.error(err)).finally(() => { this.isLoading = false; });
    }

  topScorer() {
    const allPlayers = [...this.skaters(), ...this.goalLeaders()];
    if (allPlayers.length === 0) return null;
    return allPlayers.sort((a, b) => b.points - a.points)[0];
  }

   // For the MOST ASSISTS card
  topAssistPlayer = computed(() => {
    const players = this.skaters();
    if (players.length === 0) return null;
    return [...players].sort((a, b) => b.assists - a.assists)[0];
  });

   // Top Points Player from skaters only
  topScorerPlayer() {
    const players = this.skaters();
    if (players.length === 0) return null;
    return [...players].sort((a, b) => b.points - a.points)[0];
  }

  sortAllPlayers(column: string) {
    if (this.allPlayersSortColumn() === column) {
      this.allPlayersSortDirection.update(dir => dir === 'asc' ? 'desc' : 'asc');
    } else {
      this.allPlayersSortColumn.set(column);
      this.allPlayersSortDirection.set('desc');
    }
  }

  sortedAllPlayers = computed(() => {
    // Get all players (combined from skaters and goal leaders, no duplicates)
    const playerMap = new Map<number, Skater | GoalLeader>();
    this.skaters().forEach(p => playerMap.set(p.playerId, p));
    this.goalLeaders().forEach(leader => { if (!playerMap.has(leader.playerId)) { playerMap.set(leader.playerId, leader); } });
    
    let players = Array.from(playerMap.values());
    
    // Apply search filter
    const query = this.searchQuery().toLowerCase();
    if (query) {
      players = players.filter(p => p.fullName.toLowerCase().includes(query) || p.teamName.toLowerCase().includes(query));
    }
    
    // Sort by selected column
    const column = this.allPlayersSortColumn();
    const direction = this.allPlayersSortDirection();
    
    return players.sort((a, b) => {
      let valueA: number | string;
      let valueB: number | string;
      
      switch (column) {
        case 'goals': valueA = (a as Skater).goals; valueB = (b as Skater).goals; break;
        case 'assists': valueA = (a as Skater).assists; valueB = (b as Skater).assists; break;
        default: valueA = (a as Skater).points; valueB = (b as Skater).points; break;
      }
      
      // All sort columns are numeric (goals, assists, points)
      const comparison = Number(valueA) - Number(valueB);
      return direction === 'asc' ? comparison : -comparison;
    });
  })

  getSortIndicator(column: string): string {
    if (this.allPlayersSortColumn() !== column) return '';
    return this.allPlayersSortDirection() === 'asc' ? '↑' : '↓';
  }

  getPositionLabel(player: Skater | GoalLeader): string {
    if ('positionCode' in player) { const pos = (player as any).positionCode; return pos ? pos.charAt(0).toUpperCase() : ''; }
    if ('position' in player) { const pos = player.position; return typeof pos === 'string' && pos.length > 0 ? pos.charAt(0).toUpperCase() : ''; }
    return '';
  }

  getFilteredPlayers() {
    const query = this.searchQuery().toLowerCase();
    
    if (this.activeTab() === 'skaters') {
      // Static order: just sort by points descending
      return [...this.skaters()].sort((a, b) => b.points - a.points);
    } else if (this.activeTab() === 'goalleaders') {
      let players = this.goalLeaders();
      if (query) {
        players = players.filter(p => p.fullName.toLowerCase().includes(query) || p.teamName.toLowerCase().includes(query));
      }
      return players;
    } else {
      // All Players tab
      return this.sortedAllPlayers();
    }
  }

  getAbbreviation(teamName: string): string {
    const teamMap: Record<string, string> = {
      'Boston Bruins': 'BOS', 'Chicago Blackhawks': 'CHI', 'Detroit Red Wings': 'DET',
      'Montreal Canadiens': 'MTL', 'New York Rangers': 'NYR', 'Toronto Maple Leafs': 'TOR',
      'Los Angeles Kings': 'LAK', 'Philadelphia Flyers': 'PHI', 'Pittsburgh Penguins': 'PIT', 'St. Louis Blues': 'STL',
      'Buffalo Sabres': 'BUF', 'Edmonton Oilers': 'EDM', 'Vancouver Canucks': 'VAN',
      'Anaheim Ducks': 'ANA', 'Arizona Coyotes': 'ARI', 'Carolina Hurricanes': 'CAR',
      'Columbus Blue Jackets': 'CBJ', 'Colorado Avalanche': 'COL', 'Dallas Stars': 'DAL',
      'Florida Panthers': 'FLA', 'Minnesota Wild': 'MIN', 'Nashville Predators': 'NSH',
      'New Jersey Devils': 'NJD', 'New York Islanders': 'NYI', 'Ottawa Senators': 'OTT',
      'San Jose Sharks': 'SJS', 'Seattle Kraken': 'SEA', 'Tampa Bay Lightning': 'TBL',
      'Utah Hockey Club': 'UTA', 'Vegas Golden Knights': 'VGK', 'Washington Capitals': 'WSH', 'Winnipeg Jets': 'WPG'
    };
    
    if (teamMap[teamName]) { return teamMap[teamName]; }
    const parts = teamName.split(' ');
    if (parts.length > 0) { return parts[0].substring(0, 3).toUpperCase(); }
    return teamName;
  }

  getTeamAbbrev(player: Skater | GoalLeader): string {
    if ('teamName' in player && typeof (player as Skater).teamName === 'string') { return this.getAbbreviation((player as Skater).teamName); }
    return (player as GoalLeader).teamName || '';
  }

  formatName(player: Skater | GoalLeader): string {
    if ('skaterFullName' in player) { return (player as GoalLeader & { skaterFullName: string }).skaterFullName; }
    return 'fullName' in player ? player.fullName : '';
  }

   topScorerName() {
     const scorer = this.topScorerPlayer();
     if (!scorer) return '';
     return 'skaterFullName' in scorer ? scorer.skaterFullName : scorer.fullName;
   }

   // For the MOST ASSISTS card
   getTopAssistPlayerName() {
     const player = this.topAssistPlayer();
     if (!player) return '';
     return 'skaterFullName' in player ? player.skaterFullName : player.fullName;
   }

  topGoalScorerName() {
    const scorer = this.topGoalScorer();
    if (!scorer) return '';
    return 'skaterFullName' in scorer ? scorer.skaterFullName : scorer.fullName;
  }

  // Top Goal Scorer - returns player with most goals (used for Top Goal Scorer card)
  topGoalScorer() {
    const allPlayers = [...this.skaters(), ...this.goalLeaders()];
    if (allPlayers.length === 0) return null;
    return allPlayers.sort((a, b) => b.goals - a.goals)[0];
  }
}
