import { inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { CanDeactivateFn } from '@angular/router';
import { CommonService } from '../services/common.service';
import { ConfrirmComponent } from '../components/confrirm/confrirm.component';
import { map } from 'rxjs';

export const preventUnsavedChangesGuard: CanDeactivateFn<unknown> = () => {
  const dialog = inject(MatDialog);
  const commonService = inject(CommonService);

  if (commonService.isPreveventLeavingPage) {
    const diologRef = dialog.open(ConfrirmComponent);

    return diologRef.afterClosed()
      .pipe(
        map((action: boolean) => {
          if (action) return true

          return false
        })
      );
  }

  return true;
};
