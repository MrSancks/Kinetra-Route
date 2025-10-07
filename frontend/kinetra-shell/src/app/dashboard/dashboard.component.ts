import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

import { AuthService } from '../services/auth.service';

type MicrofrontendId = 'users' | 'orders' | 'chat';

type Microfrontend = {
  id: MicrofrontendId;
  name: string;
  tech: 'React' | 'Vue' | 'Svelte';
  summary: string;
  description: string;
  url: string;
  status: 'ready' | 'coming-soon';
};

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent {
  protected readonly microfrontends: Microfrontend[] = [
    {
      id: 'users',
      name: 'Usuarios',
      tech: 'React',
      summary: 'Gestiona repartidores, restaurantes y permisos.',
      description:
        'Conecta el microfrontend de administracion de usuarios y accede a la gestion detallada de repartidores, restaurantes y roles desde React.',
      url: 'http://localhost:4201',
      status: 'ready'
    },
    {
      id: 'orders',
      name: 'Pedidos',
      tech: 'Vue',
      summary: 'Monitorea pedidos en tiempo real y optimiza rutas.',
      description:
        'Integra el microfrontend de pedidos construidos en Vue para seguir el estado de cada orden, couriers y rutas activas.',
      url: 'http://localhost:4202',
      status: 'ready'
    },
    {
      id: 'chat',
      name: 'Chat',
      tech: 'Svelte',
      summary: 'Habilita la comunicacion entre repartidores y clientes.',
      description:
        'Este microfrontend estara disponible proximamente para conectar la mensajeria en tiempo real dentro de la shell.',
      url: 'http://localhost:4203',
      status: 'coming-soon'
    }
  ];

  protected readonly safeUrls: Partial<Record<MicrofrontendId, SafeResourceUrl>>;
  protected selectedMicrofrontendId: MicrofrontendId = 'users';

  constructor(
    private readonly authService: AuthService,
    private readonly router: Router,
    private readonly sanitizer: DomSanitizer
  ) {
    this.safeUrls = this.microfrontends.reduce((acc, microfrontend) => {
      if (microfrontend.status === 'ready') {
        acc[microfrontend.id] = this.sanitizer.bypassSecurityTrustResourceUrl(microfrontend.url);
      }

      return acc;
    }, {} as Partial<Record<MicrofrontendId, SafeResourceUrl>>);
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  protected selectMicrofrontend(id: MicrofrontendId): void {
    if (this.selectedMicrofrontendId === id) {
      return;
    }

    const target = this.microfrontends.find((microfrontend) => microfrontend.id === id);
    if (!target || target.status !== 'ready') {
      return;
    }

    this.selectedMicrofrontendId = id;
  }

  protected get selectedMicrofrontend(): Microfrontend {
    return (
      this.microfrontends.find((microfrontend) => microfrontend.id === this.selectedMicrofrontendId) ??
      this.microfrontends[0]
    );
  }

  protected get selectedMicrofrontendUrl(): SafeResourceUrl | null {
    return this.safeUrls[this.selectedMicrofrontendId] ?? null;
  }
}
