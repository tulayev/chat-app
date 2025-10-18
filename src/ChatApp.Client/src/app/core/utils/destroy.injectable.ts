import { DestroyRef, Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable()
export class Destroy extends Subject<void> {
  constructor(destroyRef: DestroyRef) {
    super();
    
    destroyRef.onDestroy(() => {
      this.next();
      this.complete();
    });
  }
}
