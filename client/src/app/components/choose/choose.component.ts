import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { RouterModule } from '@angular/router';

@Component({
    selector: 'app-choose',
    imports: [
        MatCardModule, MatButtonModule,
        RouterModule
    ],
    templateUrl: './choose.component.html',
    styleUrl: './choose.component.scss'
})
export class ChooseComponent {

}
