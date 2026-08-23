import { AsyncPipe } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { UserService } from '@app/core/services';
import { User } from '@app/models';
import { map, Observable } from 'rxjs';

@Component({
  selector: 'app-users',
  imports: [AsyncPipe],
  templateUrl: './users.component.html'
})
export class UsersComponent implements OnInit {
  users$!: Observable<User[]>;
  
  private readonly userService = inject(UserService);

  ngOnInit(): void {
    this.loadData();
  }

  private loadData(): void {
    this.users$ = this.userService.getUsers().pipe(
      map((response) => response.data)
    );
  }
}
