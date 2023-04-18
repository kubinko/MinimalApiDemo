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

  getInvoiceLink(attendee: Attendee): string {
    return this.invoiceLinkEnabled(attendee)
      ? this.apiService.getInvoiceUri(attendee.id, attendee.invoiceCode)
      : "#";
  }

  invoiceLinkEnabled(attendee: Attendee): boolean {
    return attendee.invoiceCode != undefined && attendee.invoiceCode.length > 0;
  }

  deleteAttendee(id: number) {
    this.apiService.deleteAttendee(id).subscribe(_ => {
      this.attendees.forEach((element, index) => {
        if (element.id == id) this.attendees.splice(index,1);
      });
    });
  }
}
