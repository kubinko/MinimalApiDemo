import { Component, OnInit } from '@angular/core';
import { ApiService } from '../services/api.service';
import { Attendee } from '../models/attendee';

@Component({
  selector: 'app-attendee-list',
  templateUrl: './attendee-list.component.html',
  styleUrls: ['./attendee-list.component.css']
})
export class AttendeeListComponent implements OnInit {
  public attendees: Attendee[] = [];

  constructor(private apiService: ApiService) { }

  ngOnInit() {
    this.apiService.getAttendees().subscribe(data => {
      this.attendees = data;
    });
  }

}
