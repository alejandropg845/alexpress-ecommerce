import { Component } from '@angular/core';
import { LoadingDialogService } from '../../services/loading-dialog.service';

@Component({
  selector: 'app-loading-dialog',
  templateUrl: './loading-dialog.component.html',
  styles: ``
})
export class LoadingDialogComponent {

  constructor(public loadingDialogService:LoadingDialogService){}

}
