import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

// Interfaces - TypeScript types that define what our data looks like
// This is crucial for type safety and helps IDEs give you better autocomplete

interface Skater {
  playerId: number;
  skaterFullName: string;
  teamAbbreviations: string;
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
 * 
 * Key Concepts:
 * - @Component(): Decorator that marks this class as an Angular component
 *   - selector: The HTML tag name used to include this component ('app-root')
 *   - imports: Other Angular modules needed by this component
 *   - templateUrl: Path to the HTML file that defines the component's structure
 *   - styleUrl: Path to the CSS file for this component's styling
 * 
 * - inject<T>(): Function that gets a service instance from Angular's dependency injection system
 * - HttpClient: Angular's built-in service for making HTTP requests (GET, POST, PUT, DELETE)
 * - signal<T>(): Creates an observable-like value that automatically tracks dependencies
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
   * - Uses HttpClient GET method to retrieve JSON data
   * - The response is automatically typed as Skater[] due to TypeScript generics
   */
  loadSkaters() {
    this.http.get<Skater[]>('http://localhost:5048/api/nhl/leaders/skaters?limit=50')
      .subscribe({
        next: (data) => this.skaters.set(data),
        error: (err) => console.error('Error loading skaters:', err)
      });
  }

  /**
   * loadGoalLeaders(): Fetches top 20 goal scorers from the API
   * - Similar to loadSkaters but for goal leaders
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
   * - Makes POST requests to trigger syncing
   * - After successful sync, reloads the data to show updated stats
   */
  syncData() {
    this.isLoading = true;
    
    // Sync both endpoints in parallel using forkJoin pattern (using Promise.all instead)
    const syncSkaters = this.http.post<SyncResponse>('http://localhost:5048/api/sync/skaters', {})
      .toPromise();
      
    const syncGoals = this.http.post<SyncResponse>('http://localhost:5048/api/sync/goalleaders', {})
      .toPromise();

    Promise.all([syncSkaters, syncGoals])
      .then(() => {
        // Reload data after successful sync
        this.loadSkaters();
        this.loadGoalLeaders();
      })
      .catch((err) => console.error('Sync failed:', err))
      .finally(() => {
        this.isLoading = false;
      });
  }

  /**
   * getTopScorer(): Finds the player with the most points
   */
  topScorer() {
    const allPlayers = [...this.skaters(), ...this.goalLeaders()];
    if (allPlayers.length === 0) return null;
    
    // Sort by points descending and return the top one
    return allPlayers.sort((a, b) => b.points - a.points)[0];
  }

  /**
   * getTopGoalScorer(): Finds the player with the most goals
   */
  topGoalScorer() {
    const allPlayers = [...this.skaters(), ...this.goalLeaders()];
    if (allPlayers.length === 0) return null;
    
    // Sort by goals descending and return the top one
    return allPlayers.sort((a, b) => b.goals - a.goals)[0];
  }

  /**
   * getTopGoalScorerName(): Returns formatted name of top goal scorer
   */
  topGoalScorerName() {
    const scorer = this.topGoalScorer();
    if (!scorer) return '';
    
    // Handle different field names (skaters have skaterFullName, goalLeaders have firstName/lastName)
    if ('firstName' in scorer && 'lastName' in scorer) {
      return `${scorer.firstName} ${scorer.lastName}`;
    }
    return scorer.skaterFullName || scorer.skaterFullName;
  }

  /**
   * getTopScorerName(): Returns formatted name of top scorer
   */
  topScorerName() {
    const scorer = this.topScorer();
    if (!scorer) return '';
    
    // Handle different field names (skaters have skaterFullName, goalLeaders have firstName/lastName)
    if ('firstName' in scorer && 'lastName' in scorer) {
      return `${scorer.firstName} ${scorer.lastName}`;
    }
    return scorer.skaterFullName || scorer.skaterFullName;
  }

  /**
   * getTeamAbbrev(): Formats team abbreviation for display
   */
  getTeamAbbrev(player: Skater | GoalLeader): string {
    if ('teamAbbreviations' in player) {
      return player.teamAbbreviations.split(',')[0] || '';
    }
    return player.teamAbbreviation || '';
  }

  /**
   * getPositionLabel(): Returns position label with proper formatting
   */
  getPositionLabel(player: Skater | GoalLeader): string {
    if ('position' in player) {
      return player.position;
    }
    // Goal leaders might not have position, use N/A
    return 'N/A';
  }

  /**
   * formatName(): Formats player name consistently
   */
  formatName(player: Skater | GoalLeader): string {
    if ('firstName' in player && 'lastName' in player) {
      return `${player.firstName} ${player.lastName}`;
    }
    return player.skaterFullName;
  }
}