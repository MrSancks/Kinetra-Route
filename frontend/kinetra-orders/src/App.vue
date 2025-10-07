<script setup lang="ts">
import { computed, ref } from 'vue';
import dayjs from 'dayjs';
import relativeTime from 'dayjs/plugin/relativeTime';
import 'dayjs/locale/es';

dayjs.extend(relativeTime);
dayjs.locale('es');

type OrderStatus = 'Preparando' | 'Listo para envío' | 'En camino';

type Order = {
  id: string;
  restaurant: string;
  client: string;
  zone: string;
  status: OrderStatus;
  etaMinutes: number;
  courier: string;
  placedAt: string;
  distanceKm: number;
};

type RouteStop = {
  label: string;
  eta: string;
  status: 'pendiente' | 'en-progreso' | 'completado';
};

type DeliveryRoute = {
  id: string;
  courier: string;
  startedAt: string;
  vehicle: 'Moto' | 'Bicicleta' | 'Auto';
  totalDistanceKm: number;
  progress: number;
  nextStop: string;
  stops: RouteStop[];
};

const orders = ref<Order[]>([
  {
    id: 'ORD-1045',
    restaurant: 'La Parrilla Urbana',
    client: 'Ana Ríos',
    zone: 'Centro',
    status: 'En camino',
    etaMinutes: 12,
    courier: 'Laura Méndez',
    placedAt: '2024-04-04T17:10:00Z',
    distanceKm: 4.2
  },
  {
    id: 'ORD-1046',
    restaurant: 'Sushi Río',
    client: 'Carlos Navarro',
    zone: 'Norte',
    status: 'Listo para envío',
    etaMinutes: 18,
    courier: 'Diego Patiño',
    placedAt: '2024-04-04T17:15:00Z',
    distanceKm: 6.5
  },
  {
    id: 'ORD-1047',
    restaurant: 'Arepa House',
    client: 'Fernanda Ortiz',
    zone: 'Sur',
    status: 'Preparando',
    etaMinutes: 24,
    courier: 'Equipo Cocina',
    placedAt: '2024-04-04T17:22:00Z',
    distanceKm: 3.1
  },
  {
    id: 'ORD-1048',
    restaurant: 'Pizza Nómada',
    client: 'Luis Romero',
    zone: 'Centro',
    status: 'En camino',
    etaMinutes: 9,
    courier: 'Laura Méndez',
    placedAt: '2024-04-04T17:28:00Z',
    distanceKm: 2.4
  }
]);

const routes = ref<DeliveryRoute[]>([
  {
    id: 'Ruta 12',
    courier: 'Laura Méndez',
    startedAt: '2024-04-04T17:05:00Z',
    vehicle: 'Moto',
    totalDistanceKm: 8.3,
    progress: 72,
    nextStop: 'ORD-1048 · Torre Nova, Centro',
    stops: [
      { label: 'Base Centro', eta: '17:05', status: 'completado' },
      { label: 'La Parrilla Urbana', eta: '17:18', status: 'completado' },
      { label: 'Ana Ríos', eta: '17:30', status: 'en-progreso' },
      { label: 'Pizza Nómada', eta: '17:34', status: 'pendiente' },
      { label: 'Luis Romero', eta: '17:42', status: 'pendiente' }
    ]
  },
  {
    id: 'Ruta 7',
    courier: 'Diego Patiño',
    startedAt: '2024-04-04T17:12:00Z',
    vehicle: 'Bicicleta',
    totalDistanceKm: 5.7,
    progress: 48,
    nextStop: 'ORD-1046 · Corporativo Norte',
    stops: [
      { label: 'Base Norte', eta: '17:12', status: 'completado' },
      { label: 'Sushi Río', eta: '17:19', status: 'en-progreso' },
      { label: 'Carlos Navarro', eta: '17:33', status: 'pendiente' },
      { label: 'Recolección express', eta: '17:38', status: 'pendiente' }
    ]
  },
  {
    id: 'Ruta Cocina',
    courier: 'Equipo Cocina',
    startedAt: '2024-04-04T16:58:00Z',
    vehicle: 'Auto',
    totalDistanceKm: 9.4,
    progress: 31,
    nextStop: 'ORD-1047 · Preparación en curso',
    stops: [
      { label: 'Centro logístico', eta: '16:58', status: 'completado' },
      { label: 'Arepa House', eta: '17:10', status: 'en-progreso' },
      { label: 'Fernanda Ortiz', eta: '17:36', status: 'pendiente' },
      { label: 'Nueva solicitud Sur', eta: '17:48', status: 'pendiente' }
    ]
  }
]);

const lastSync = dayjs().subtract(3, 'minute');

const totalActiveOrders = computed(() => orders.value.length);

const couriersOnRoute = computed(() => {
  const uniqueCouriers = new Set(orders.value.map((order) => order.courier));
  return uniqueCouriers.size;
});

const averageEta = computed(() => {
  if (orders.value.length === 0) {
    return 0;
  }

  const totalMinutes = orders.value.reduce((acc, order) => acc + order.etaMinutes, 0);
  return Math.round(totalMinutes / orders.value.length);
});

const totalDistance = computed(() =>
  orders.value.reduce((acc, order) => acc + order.distanceKm, 0).toFixed(1)
);

const delayedOrders = computed(() => orders.value.filter((order) => order.etaMinutes >= 20).length);

const lastSyncLabel = computed(() => lastSync.fromNow());

const statusStyles: Record<OrderStatus, string> = {
  Preparando: 'status status--preparing',
  'Listo para envío': 'status status--ready',
  'En camino': 'status status--on-way'
};

function formatTime(isoString: string): string {
  return dayjs(isoString).format('HH:mm');
}
</script>

<template>
  <main class="layout">
    <header class="hero">
      <div>
        <p class="hero__eyebrow">Kinetra Orders</p>
        <h1>Monitoreo de pedidos activos y rutas</h1>
        <p class="hero__description">
          Supervisa en tiempo real el estado de cada pedido, asignación de repartidores y próximas
          entregas para mantener a tus clientes informados.
        </p>
      </div>
      <div class="hero__timestamp">
        <span class="dot"></span>
        <span>Actualizado {{ lastSyncLabel }}</span>
      </div>
    </header>

    <section class="stats">
      <article class="stat-card">
        <p class="stat-card__label">Pedidos activos</p>
        <p class="stat-card__value">{{ totalActiveOrders }}</p>
        <p class="stat-card__helper">Incluye preparación y entregas en curso</p>
      </article>
      <article class="stat-card">
        <p class="stat-card__label">Repartidores en ruta</p>
        <p class="stat-card__value">{{ couriersOnRoute }}</p>
        <p class="stat-card__helper">Cubre zonas Centro, Norte y Sur</p>
      </article>
      <article class="stat-card">
        <p class="stat-card__label">ETA promedio</p>
        <p class="stat-card__value">{{ averageEta }}&prime;</p>
        <p class="stat-card__helper">Basado en los pedidos asignados</p>
      </article>
      <article class="stat-card">
        <p class="stat-card__label">Distancia acumulada</p>
        <p class="stat-card__value">{{ totalDistance }} km</p>
        <p class="stat-card__helper">Proyectada para la ventana actual</p>
      </article>
    </section>

    <section class="panel">
      <header class="panel__header">
        <div>
          <h2>Pedidos activos</h2>
          <p>{{ delayedOrders }} pedidos con ETA mayor a 20 minutos</p>
        </div>
        <button class="panel__cta" type="button">Exportar lista</button>
      </header>

      <div class="table-wrapper">
        <table>
          <thead>
            <tr>
              <th>Pedido</th>
              <th>Restaurante</th>
              <th>Cliente</th>
              <th>ZONA</th>
              <th>Estado</th>
              <th>ETA</th>
              <th>Repartidor</th>
              <th>Creado</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="order in orders" :key="order.id">
              <td>
                <span class="order-id">{{ order.id }}</span>
                <span class="order-distance">{{ order.distanceKm.toFixed(1) }} km</span>
              </td>
              <td>
                <p class="table__title">{{ order.restaurant }}</p>
                <p class="table__subtitle">{{ order.zone }} · cocina</p>
              </td>
              <td>{{ order.client }}</td>
              <td>{{ order.zone }}</td>
              <td>
                <span :class="statusStyles[order.status]">{{ order.status }}</span>
              </td>
              <td>
                <strong>{{ order.etaMinutes }}&prime;</strong>
              </td>
              <td>{{ order.courier }}</td>
              <td>{{ formatTime(order.placedAt) }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </section>

    <section class="panel">
      <header class="panel__header">
        <div>
          <h2>Rutas en ejecución</h2>
          <p>Control de hitos y proximas paradas</p>
        </div>
        <button class="panel__cta" type="button">Ver mapa</button>
      </header>

      <div class="routes">
        <article v-for="route in routes" :key="route.id" class="route-card">
          <header class="route-card__header">
            <div>
              <p class="route-card__eyebrow">{{ route.id }}</p>
              <h3>{{ route.courier }}</h3>
            </div>
            <div class="route-card__meta">
              <span>{{ route.vehicle }}</span>
              <span>Inicio {{ formatTime(route.startedAt) }}</span>
            </div>
          </header>

          <div class="route-card__progress">
            <div class="route-card__progress-bar">
              <span :style="{ width: `${route.progress}%` }"></span>
            </div>
            <p>{{ route.progress }}% completado · Próxima parada: {{ route.nextStop }}</p>
          </div>

          <ul class="route-card__stops">
            <li v-for="stop in route.stops" :key="`${route.id}-${stop.label}`" :class="[`stop`, `stop--${stop.status}`]">
              <div class="stop__indicator"></div>
              <div>
                <p class="stop__label">{{ stop.label }}</p>
                <p class="stop__eta">{{ stop.eta }}</p>
              </div>
            </li>
          </ul>

          <footer class="route-card__footer">
            <span>{{ route.totalDistanceKm.toFixed(1) }} km previstos</span>
            <button type="button">Reasignar</button>
          </footer>
        </article>
      </div>
    </section>
  </main>
</template>

<style scoped lang="scss">
.layout {
  display: flex;
  flex-direction: column;
  gap: 2.5rem;
}

.hero {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  justify-content: space-between;
  gap: 1.5rem;
  background: #fff;
  border-radius: 1.5rem;
  padding: 2rem 2.5rem;
  box-shadow: 0 24px 60px -40px rgba(19, 19, 26, 0.45);
}

.hero__eyebrow {
  font-size: 0.9rem;
  text-transform: uppercase;
  letter-spacing: 0.08em;
  color: #4c6ef5;
  font-weight: 600;
}

.hero h1 {
  font-size: 2.25rem;
  margin-top: 0.3rem;
  margin-bottom: 0.8rem;
}

.hero__description {
  max-width: 36rem;
  color: #606274;
}

.hero__timestamp {
  display: inline-flex;
  align-items: center;
  gap: 0.6rem;
  background: #f1f5ff;
  padding: 0.75rem 1.2rem;
  border-radius: 999px;
  color: #3b5bdb;
  font-weight: 600;
  font-size: 0.95rem;
}

.dot {
  width: 10px;
  height: 10px;
  border-radius: 999px;
  background: radial-gradient(circle at 30% 30%, #51cf66, #2f9e44);
  box-shadow: 0 0 0 6px rgba(81, 207, 102, 0.18);
}

.stats {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
  gap: 1.25rem;
}

.stat-card {
  background: linear-gradient(160deg, #ffffff, #f4f7ff 85%);
  border-radius: 1.25rem;
  padding: 1.6rem;
  box-shadow: 0 18px 45px -35px rgba(27, 35, 66, 0.6);
}

.stat-card__label {
  font-size: 0.95rem;
  color: #4b4d63;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.06em;
}

.stat-card__value {
  font-size: 2rem;
  font-weight: 700;
  margin: 0.4rem 0;
  color: #1b2559;
}

.stat-card__helper {
  color: #7a7d96;
  font-size: 0.95rem;
}

.panel {
  background: #fff;
  border-radius: 1.5rem;
  padding: 2rem 2.25rem;
  box-shadow: 0 24px 55px -40px rgba(22, 23, 31, 0.55);
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.panel__header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 1rem;
}

.panel__header h2 {
  font-size: 1.5rem;
  margin-bottom: 0.35rem;
}

.panel__header p {
  color: #6c6f85;
}

.panel__cta {
  background: #4c6ef5;
  color: white;
  border: none;
  border-radius: 999px;
  padding: 0.65rem 1.4rem;
  font-weight: 600;
  cursor: pointer;
  transition: background 0.2s ease;
}

.panel__cta:hover {
  background: #3b5bdb;
}

.table-wrapper {
  overflow-x: auto;
}

table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.95rem;
}

th {
  text-align: left;
  font-size: 0.75rem;
  font-weight: 700;
  letter-spacing: 0.08em;
  text-transform: uppercase;
  color: #7b7f9e;
  padding-bottom: 1rem;
}

td {
  padding: 1.05rem 0;
  border-top: 1px solid #edf0f7;
  vertical-align: middle;
}

tr:first-child td {
  border-top: none;
}

.order-id {
  font-weight: 700;
  color: #1f2555;
  display: block;
}

.order-distance {
  color: #868ab5;
  font-size: 0.85rem;
}

.table__title {
  font-weight: 600;
  color: #1f2555;
}

.table__subtitle {
  color: #9498c4;
  font-size: 0.85rem;
}

.status {
  display: inline-flex;
  align-items: center;
  gap: 0.4rem;
  border-radius: 999px;
  padding: 0.35rem 0.85rem;
  font-weight: 600;
  text-transform: uppercase;
  font-size: 0.7rem;
  letter-spacing: 0.08em;
}

.status::before {
  content: '';
  width: 8px;
  height: 8px;
  border-radius: 999px;
}

.status--preparing {
  background: rgba(255, 193, 7, 0.15);
  color: #a07800;
}

.status--preparing::before {
  background: #ffc107;
}

.status--ready {
  background: rgba(76, 175, 80, 0.18);
  color: #2b8a3e;
}

.status--ready::before {
  background: #4caf50;
}

.status--on-way {
  background: rgba(76, 110, 245, 0.18);
  color: #2648d1;
}

.status--on-way::before {
  background: #4c6ef5;
}

.routes {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(260px, 1fr));
  gap: 1.5rem;
}

.route-card {
  border: 1px solid #e7e9f6;
  border-radius: 1.25rem;
  padding: 1.5rem;
  display: flex;
  flex-direction: column;
  gap: 1.3rem;
  background: linear-gradient(180deg, rgba(247, 249, 255, 0.9) 0%, #ffffff 60%);
}

.route-card__header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: 0.75rem;
}

.route-card__eyebrow {
  text-transform: uppercase;
  color: #6f73a6;
  font-weight: 600;
  font-size: 0.75rem;
  letter-spacing: 0.08em;
}

.route-card__header h3 {
  font-size: 1.35rem;
  color: #1f2555;
}

.route-card__meta {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 0.35rem;
  color: #7c81b0;
  font-weight: 600;
}

.route-card__progress {
  display: flex;
  flex-direction: column;
  gap: 0.4rem;
  color: #636890;
  font-size: 0.9rem;
}

.route-card__progress-bar {
  position: relative;
  width: 100%;
  height: 8px;
  background: #ecefff;
  border-radius: 999px;
  overflow: hidden;
}

.route-card__progress-bar span {
  position: absolute;
  left: 0;
  top: 0;
  bottom: 0;
  border-radius: 999px;
  background: linear-gradient(90deg, #4c6ef5, #74c0fc);
}

.route-card__stops {
  list-style: none;
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.stop {
  display: flex;
  align-items: flex-start;
  gap: 0.75rem;
}

.stop__indicator {
  width: 12px;
  height: 12px;
  border-radius: 999px;
  margin-top: 0.3rem;
  border: 3px solid transparent;
}

.stop__label {
  font-weight: 600;
  color: #232755;
}

.stop__eta {
  color: #8a8ebd;
  font-size: 0.85rem;
}

.stop--completado .stop__indicator {
  background: #4caf50;
  border-color: rgba(76, 175, 80, 0.25);
}

.stop--en-progreso .stop__indicator {
  background: #4c6ef5;
  border-color: rgba(76, 110, 245, 0.25);
}

.stop--pendiente .stop__indicator {
  background: #d0d4ff;
  border-color: rgba(208, 212, 255, 0.25);
}

.route-card__footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
  font-size: 0.9rem;
  color: #6e7396;
}

.route-card__footer button {
  border: none;
  background: transparent;
  color: #4c6ef5;
  font-weight: 600;
  cursor: pointer;
}

@media (max-width: 768px) {
  .panel__header {
    flex-direction: column;
    align-items: flex-start;
  }

  .panel__cta {
    width: 100%;
  }

  th,
  td {
    padding-right: 0.75rem;
  }
}
</style>
