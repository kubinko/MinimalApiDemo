import { Component, OnInit } from '@angular/core';
import { ApiService } from './services/api.service';
import { Workshop } from './models/workshop';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Attendance';
  workshop: Workshop = {
    name: '',
    price: 0
  };

  constructor(private apiService: ApiService) { }

  ngOnInit() {
    this.apiService.getWorkshopInfo().subscribe(data => {
      this.workshop = data;
    });
  }
}
