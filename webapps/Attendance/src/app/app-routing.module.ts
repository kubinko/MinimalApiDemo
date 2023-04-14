import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AttendeeListComponent } from './attendee-list/attendee-list.component';
import { NewAttendeeComponent } from './new-attendee/new-attendee.component';

const routes: Routes = [
  { path: '', redirectTo: '/list', pathMatch: 'full' },
  { path: 'add', component: NewAttendeeComponent },
  { path: 'list', component: AttendeeListComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
