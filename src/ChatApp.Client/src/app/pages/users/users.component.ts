import { AsyncPipe } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { AuthService, UserService } from '@app/core/services';
import { User } from '@app/models';
import { Router, RouterLink } from '@angular/router';
import { catchError, debounceTime, distinctUntilChanged, map, Observable, of, startWith, switchMap } from 'rxjs';
import { LucideLogOut, LucideMessageCircle, LucideSearch } from '@lucide/angular';
import { AvatarComponent } from '@shared/components';
import { FormControl, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-users',
  imports: [AsyncPipe, RouterLink, ReactiveFormsModule, AvatarComponent, LucideLogOut, LucideMessageCircle, LucideSearch],
  templateUrl: './users.component.html'
})
export class UsersComponent implements OnInit {
  users$!: Observable<User[]>;
  searchBox = new FormControl('');

  private readonly userService = inject(UserService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly currentUser = this.authService.user;

  ngOnInit(): void {
    this.loadData();
  }

  onLogout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  private loadData(): void {
    this.users$ = this.searchBox.valueChanges.pipe(
      startWith(''),
      debounceTime(300),
      distinctUntilChanged(),
      switchMap(searchTerm => {
        if (!searchTerm || searchTerm.trim() === '') {
          return this.userService.getUsers().pipe(
            map(response => response.data),
            catchError(() => of([]))
          );
        }
        
        return this.userService.searchUser(searchTerm).pipe(
          map(response => response.data),
          catchError(() => of([]))
        );
      })
    )
  }
}
