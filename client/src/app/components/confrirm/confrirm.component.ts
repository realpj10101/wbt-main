import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { CommonService } from '../../services/common.service';

@Component({
    selector: 'app-confrirm',
    imports: [
        MatButtonModule, MatDialogModule
    ],
    templateUrl: './confrirm.component.html',
    styleUrl: './confrirm.component.scss'
})
export class ConfrirmComponent {
}
