import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

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
  skaterSortColumn = signal('points');
  skaterSortDirection = signal<'asc' | 'desc'>('desc');

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

  topGoalScorer() {
    const allPlayers = [...this.skaters(), ...this.goalLeaders()];
    if (allPlayers.length === 0) return null;
    return allPlayers.sort((a, b) => b.goals - a.goals)[0];
  }

  topAssistPlayer() {
    const players = this.skaters();
    if (players.length === 0) return null;
    return [...players].sort((a, b) => b.assists - a.assists)[0];
  }

  sortSkaters(column: string) {
    if (this.skaterSortColumn() === column) {
      this.skaterSortDirection.update(dir => dir === 'asc' ? 'desc' : 'asc');
    } else {
      this.skaterSortColumn.set(column);
      this.skaterSortDirection.set('desc');
    }
  }

  sortedSkaters = computed(() => {
    let players: (Skater | GoalLeader)[] = [];
    if (this.activeTab() === 'skaters') { players = this.skaters(); }
    else if (this.activeTab() === 'goalleaders') { players = this.goalLeaders(); }
    else {
      const playerMap = new Map<number, Skater | GoalLeader>();
      this.skaters().forEach(p => playerMap.set(p.playerId, p));
      this.goalLeaders().forEach(leader => { if (!playerMap.has(leader.playerId)) { playerMap.set(leader.playerId, leader); } });
      players = Array.from(playerMap.values());
    }
    
    const query = this.searchQuery().toLowerCase();
    let filteredPlayers: (Skater | GoalLeader)[];
    if (query) {
      filteredPlayers = players.filter(p => p.fullName.toLowerCase().includes(query) || p.teamName.toLowerCase().includes(query));
    } else {
      filteredPlayers = [...players];
    }
    
    if (this.activeTab() !== 'skaters') { return filteredPlayers; }
    
    const column = this.skaterSortColumn();
    const direction = this.skaterSortDirection();
    
    return filteredPlayers.sort((a, b) => {
      let valueA: number | string;
      let valueB: number | string;
      
      switch (column) {
        case 'name': valueA = a.fullName.toLowerCase(); valueB = b.fullName.toLowerCase(); break;
        case 'team': valueA = a.teamName.toLowerCase(); valueB = b.teamName.toLowerCase(); break;
        case 'pos': valueA = (a.position || '').toLowerCase(); valueB = (b.position || '').toLowerCase(); break;
        case 'goals': valueA = (a as Skater).goals; valueB = (b as Skater).goals; break;
        case 'assists': valueA = (a as Skater).assists; valueB = (b as Skater).assists; break;
        default: valueA = (a as Skater).points; valueB = (b as Skater).points; break;
      }
      
      if (typeof valueA === 'string' && typeof valueB === 'string') {
        const comparison = valueA.localeCompare(valueB);
        return direction === 'asc' ? comparison : -comparison;
      } else {
        const comparison = Number(valueA) - Number(valueB);
        return direction === 'asc' ? comparison : -comparison;
      }
    });
  })

  getSortIndicator(column: string): string {
    if (this.skaterSortColumn() !== column) return '';
    return this.skaterSortDirection() === 'asc' ? '↑' : '↓';
  }

  getPositionLabel(player: Skater | GoalLeader): string {
    if ('positionCode' in player) { const pos = (player as any).positionCode; return pos ? pos.charAt(0).toUpperCase() : ''; }
    if ('position' in player) { const pos = player.position; return typeof pos === 'string' && pos.length > 0 ? pos.charAt(0).toUpperCase() : ''; }
    return '';
  }

  getAllPlayers() {
    const playerMap = new Map<number, Skater | GoalLeader>();
    this.skaters().forEach(player => playerMap.set(player.playerId, player));
    this.goalLeaders().forEach(leader => { if (!playerMap.has(leader.playerId)) { playerMap.set(leader.playerId, leader); } });
    return Array.from(playerMap.values()).sort((a, b) => (b as any).points - (a as any).points);
  }

  getFilteredPlayers() {
    const query = this.searchQuery().toLowerCase();
    let allPlayers: (Skater | GoalLeader)[];
    if (this.activeTab() === 'skaters') { return this.sortedSkaters(); }
    else if (this.activeTab() === 'goalleaders') { allPlayers = this.goalLeaders(); }
    else { allPlayers = this.getAllPlayers(); }
    
    if (!query) { return allPlayers; }
    return allPlayers.filter(player => player.fullName.toLowerCase().includes(query) || player.teamName.toLowerCase().includes(query));
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
    const scorer = this.topScorer();
    if (!scorer) return '';
    return 'skaterFullName' in scorer ? scorer.skaterFullName : scorer.fullName;
  }

  topGoalScorerName() {
    const scorer = this.topGoalScorer();
    if (!scorer) return '';
    return 'skaterFullName' in scorer ? scorer.skaterFullName : scorer.fullName;
  }
}