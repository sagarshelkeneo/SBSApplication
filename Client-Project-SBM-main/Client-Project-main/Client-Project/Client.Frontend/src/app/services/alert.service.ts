import { Injectable } from '@angular/core';
import Swal from 'sweetalert2'


@Injectable({
  providedIn: 'root'
})
export class AlertService {
  constructor() { }
  Toast = Swal.mixin({
    toast: true,
    position: 'top-end',
    iconColor: 'white',
    customClass: {
      popup: 'colored-toast',
    },
    showConfirmButton: false,
    timer: 1500,
    timerProgressBar: true,
  })

  Delete = Swal.mixin({
    width: '300px',
    padding: '1em',
    showCancelButton: true,
    confirmButtonText: 'Yes',
    cancelButtonText: 'No',
    title:'Are you sure ?',
    customClass: {
      popup: 'small-confirm-popup'
    }
  });
}