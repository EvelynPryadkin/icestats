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
  firstName: string;
  lastName: string;
  teamAbbreviation: string;
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
   * getTeamAbbrev(): Formats team abbreviation for display
   */
  getTeamAbbrev(player: Skater | GoalLeader): string {
    if ('teamName' in player) {
      const parts = (player as Skater).teamName.split(' ');
      return parts.length > 1 ? parts[parts.length - 1] : (player as Skater).teamName;
    }
    return (player as GoalLeader).teamAbbreviation || '';
  }

  /**
   * getPositionLabel(): Returns position label with proper formatting
   */
  getPositionLabel(player: Skater | GoalLeader): string {
    if ('position' in player) {
      return (player as Skater).position;
    }
    return 'N/A';
  }

  /**
   * formatName(): Formats player name consistently
   */
  formatName(player: Skater | GoalLeader): string {
    if ('firstName' in player && 'lastName' in player) {
      return `${(player as GoalLeader).firstName} ${(player as GoalLeader).lastName}`;
    }
    return (player as Skater).fullName;
  }

  /**
   * topScorerName(): Returns formatted name of top scorer
   */
  topScorerName() {
    const scorer = this.topScorer();
    if (!scorer) return '';
    
    if ('firstName' in scorer && 'lastName' in scorer) {
      return `${(scorer as GoalLeader).firstName} ${(scorer as GoalLeader).lastName}`;
    }
    return (scorer as Skater).fullName;
  }

  /**
   * topGoalScorerName(): Returns formatted name of top goal scorer
   */
  topGoalScorerName() {
    const scorer = this.topGoalScorer();
    if (!scorer) return '';
    
    if ('firstName' in scorer && 'lastName' in scorer) {
      return `${(scorer as GoalLeader).firstName} ${(scorer as GoalLeader).lastName}`;
    }
    return (scorer as Skater).fullName;
  }
}