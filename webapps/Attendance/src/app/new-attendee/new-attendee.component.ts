import { Component, OnInit } from '@angular/core';
import { ApiService } from '../services/api.service';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-new-attendee',
  templateUrl: './new-attendee.component.html',
  styleUrls: ['./new-attendee.component.css']
})
export class NewAttendeeComponent implements OnInit {
  public addAttendeeForm: FormGroup;

  constructor(private apiService: ApiService, private fb : FormBuilder) {
    this.addAttendeeForm = fb.group({
      name: new FormControl('', [Validators.required, Validators.maxLength(255)]),
      email: new FormControl('', [Validators.required, Validators.maxLength(255), Validators.email]),
      birthYear: new FormControl('', [Validators.required, Validators.min(1900), Validators.max(2100)]),
    });
  }

  ngOnInit() {

  }

  saveNewAttendee() {
    this.apiService.addAttendee(this.addAttendeeForm.value).subscribe();
    this.addAttendeeForm.reset();
  }

  get name() { return this.addAttendeeForm.get('name'); }

  get email() { return this.addAttendeeForm.get('email'); }

  get birthYear() { return this.addAttendeeForm.get('birthYear'); }
}
