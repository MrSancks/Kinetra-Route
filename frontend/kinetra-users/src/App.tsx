import { useMemo, useState } from 'react';
import { useForm } from 'react-hook-form';
import {
  ColumnDef,
  createColumnHelper,
  flexRender,
  getCoreRowModel,
  useReactTable
} from '@tanstack/react-table';
import './App.css';

type CourierStatus = 'Activo' | 'En descanso';

type Courier = {
  id: string;
  name: string;
  zone: string;
  status: CourierStatus;
  rating: number;
  deliveries: number;
};

type Restaurant = {
  id: string;
  name: string;
  specialty: string;
  city: string;
  activeOrders: number;
};

type CourierFormValues = Omit<Courier, 'id'>;
type RestaurantFormValues = Omit<Restaurant, 'id'>;

const courierSeed: Courier[] = [
  {
    id: 'c-1',
    name: 'Laura Méndez',
    zone: 'Centro',
    status: 'Activo',
    rating: 4.8,
    deliveries: 32
  },
  {
    id: 'c-2',
    name: 'Juan Torres',
    zone: 'Norte',
    status: 'En descanso',
    rating: 4.5,
    deliveries: 21
  },
  {
    id: 'c-3',
    name: 'Sofía Aguilar',
    zone: 'Sur',
    status: 'Activo',
    rating: 4.9,
    deliveries: 40
  }
];

const restaurantSeed: Restaurant[] = [
  {
    id: 'r-1',
    name: 'La Parrilla Urbana',
    specialty: 'Parrilla',
    city: 'Guadalajara',
    activeOrders: 6
  },
  {
    id: 'r-2',
    name: 'Sushi Río',
    specialty: 'Sushi',
    city: 'Monterrey',
    activeOrders: 4
  },
  {
    id: 'r-3',
    name: 'Arepa House',
    specialty: 'Comida venezolana',
    city: 'CDMX',
    activeOrders: 9
  }
];

const courierColumnHelper = createColumnHelper<Courier>();
const restaurantColumnHelper = createColumnHelper<Restaurant>();

function SummaryCard({ title, value, helper }: { title: string; value: string; helper?: string }) {
  return (
    <article className="summary-card">
      <p className="summary-card__label">{title}</p>
      <p className="summary-card__value">{value}</p>
      {helper ? <p className="summary-card__helper">{helper}</p> : null}
    </article>
  );
}

type DataTableProps<TData> = {
  title: string;
  columns: ColumnDef<TData, any>[];
  data: TData[];
  emptyMessage: string;
};

function DataTable<TData>({ title, columns, data, emptyMessage }: DataTableProps<TData>) {
  const table = useReactTable({ columns, data, getCoreRowModel: getCoreRowModel() });

  return (
    <section className="panel">
      <header className="panel__header">
        <h2>{title}</h2>
        <p>{data.length} registros</p>
      </header>
      <div className="panel__table-wrapper">
        <table>
          <thead>
            {table.getHeaderGroups().map((headerGroup) => (
              <tr key={headerGroup.id}>
                {headerGroup.headers.map((header) => (
                  <th key={header.id} style={{ width: header.getSize() }}>
                    {header.isPlaceholder ? null : flexRender(header.column.columnDef.header, header.getContext())}
                  </th>
                ))}
              </tr>
            ))}
          </thead>
          <tbody>
            {table.getRowModel().rows.length === 0 ? (
              <tr>
                <td className="panel__empty" colSpan={columns.length}>
                  {emptyMessage}
                </td>
              </tr>
            ) : (
              table.getRowModel().rows.map((row) => (
                <tr key={row.id}>
                  {row.getVisibleCells().map((cell) => (
                    <td key={cell.id}>{flexRender(cell.column.columnDef.cell, cell.getContext())}</td>
                  ))}
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </section>
  );
}

type CourierFormProps = {
  onSubmit: (values: CourierFormValues) => void;
};

type RestaurantFormProps = {
  onSubmit: (values: RestaurantFormValues) => void;
};

function CourierForm({ onSubmit }: CourierFormProps) {
  const { handleSubmit, register, reset } = useForm<CourierFormValues>({
    defaultValues: {
      name: '',
      zone: 'Centro',
      status: 'Activo',
      rating: 4.5,
      deliveries: 0
    }
  });

  return (
    <form
      className="entity-form"
      onSubmit={handleSubmit((values) => {
        onSubmit({ ...values, rating: Number(values.rating), deliveries: Number(values.deliveries) });
        reset();
      })}
    >
      <h3>Nuevo repartidor</h3>
      <label>
        Nombre completo
        <input {...register('name', { required: true })} placeholder="Ej. Ana López" />
      </label>
      <label>
        Zona
        <select {...register('zone')}>
          <option value="Centro">Centro</option>
          <option value="Norte">Norte</option>
          <option value="Sur">Sur</option>
          <option value="Poniente">Poniente</option>
        </select>
      </label>
      <label>
        Estado
        <select {...register('status')}>
          <option value="Activo">Activo</option>
          <option value="En descanso">En descanso</option>
        </select>
      </label>
      <label>
        Calificación promedio
        <input type="number" step="0.1" min="0" max="5" {...register('rating', { valueAsNumber: true })} />
      </label>
      <label>
        Entregas completadas
        <input type="number" min="0" {...register('deliveries', { valueAsNumber: true })} />
      </label>
      <button type="submit">Agregar repartidor</button>
    </form>
  );
}

function RestaurantForm({ onSubmit }: RestaurantFormProps) {
  const { handleSubmit, register, reset } = useForm<RestaurantFormValues>({
    defaultValues: {
      name: '',
      specialty: 'Comida casera',
      city: 'CDMX',
      activeOrders: 0
    }
  });

  return (
    <form
      className="entity-form"
      onSubmit={handleSubmit((values) => {
        onSubmit({ ...values, activeOrders: Number(values.activeOrders) });
        reset();
      })}
    >
      <h3>Nuevo restaurante</h3>
      <label>
        Nombre comercial
        <input {...register('name', { required: true })} placeholder="Ej. Taquería Sol" />
      </label>
      <label>
        Especialidad
        <input {...register('specialty', { required: true })} placeholder="Ej. Tacos al pastor" />
      </label>
      <label>
        Ciudad
        <input {...register('city', { required: true })} placeholder="Ej. Puebla" />
      </label>
      <label>
        Pedidos activos
        <input type="number" min="0" {...register('activeOrders', { valueAsNumber: true })} />
      </label>
      <button type="submit">Agregar restaurante</button>
    </form>
  );
}

export default function App() {
  const [couriers, setCouriers] = useState<Courier[]>(courierSeed);
  const [restaurants, setRestaurants] = useState<Restaurant[]>(restaurantSeed);

  const courierColumns = useMemo(
    () => [
      courierColumnHelper.accessor('name', {
        header: 'Repartidor',
        cell: (info) => <span className="cell-primary">{info.getValue()}</span>
      }),
      courierColumnHelper.accessor('zone', {
        header: 'Zona',
        cell: (info) => <span className="chip chip--blue">{info.getValue()}</span>
      }),
      courierColumnHelper.accessor('status', {
        header: 'Estado',
        cell: (info) => (
          <span className={`chip ${info.getValue() === 'Activo' ? 'chip--green' : 'chip--amber'}`}>
            {info.getValue()}
          </span>
        )
      }),
      courierColumnHelper.accessor('rating', {
        header: 'Rating',
        cell: (info) => `${info.getValue().toFixed(1)} ⭐`
      }),
      courierColumnHelper.accessor('deliveries', {
        header: 'Entregas',
        cell: (info) => info.getValue()
      })
    ],
    []
  );

  const restaurantColumns = useMemo(
    () => [
      restaurantColumnHelper.accessor('name', {
        header: 'Restaurante',
        cell: (info) => <span className="cell-primary">{info.getValue()}</span>
      }),
      restaurantColumnHelper.accessor('specialty', {
        header: 'Especialidad'
      }),
      restaurantColumnHelper.accessor('city', {
        header: 'Ciudad'
      }),
      restaurantColumnHelper.accessor('activeOrders', {
        header: 'Pedidos activos',
        cell: (info) => (
          <span className="chip chip--purple">{info.getValue()}</span>
        )
      })
    ],
    []
  );

  const totalActiveCouriers = couriers.filter((courier) => courier.status === 'Activo').length;
  const totalDeliveries = couriers.reduce((acc, courier) => acc + courier.deliveries, 0);
  const averageRating = couriers.reduce((acc, courier) => acc + courier.rating, 0) / couriers.length;
  const totalActiveOrders = restaurants.reduce((acc, restaurant) => acc + restaurant.activeOrders, 0);

  return (
    <div className="app-shell">
      <header className="app-shell__header">
        <div>
          <p className="app-shell__eyebrow">Panel de operaciones</p>
          <h1>Gestión de repartidores y restaurantes</h1>
        </div>
        <p className="app-shell__helper">
          Controla tu red logística y de aliados comerciales en un solo lugar. Actualiza información en tiempo real para
          mantener las rutas y la disponibilidad siempre optimizadas.
        </p>
      </header>

      <section className="summary-grid">
        <SummaryCard title="Repartidores activos" value={totalActiveCouriers.toString()} helper="Disponibles en este momento" />
        <SummaryCard title="Calificación promedio" value={`${averageRating.toFixed(1)} / 5`} helper="Basado en los últimos 30 días" />
        <SummaryCard title="Entregas mensuales" value={totalDeliveries.toString()} helper="Completadas por la flota actual" />
        <SummaryCard title="Pedidos en preparación" value={totalActiveOrders.toString()} helper="Distribuidos entre restaurantes" />
      </section>

      <section className="grid">
        <div className="grid__main">
          <DataTable
            title="Repartidores"
            columns={courierColumns}
            data={couriers}
            emptyMessage="Aún no has registrado repartidores."
          />
          <DataTable
            title="Restaurantes"
            columns={restaurantColumns}
            data={restaurants}
            emptyMessage="Aún no has registrado restaurantes."
          />
        </div>
        <div className="grid__sidebar">
          <CourierForm
            onSubmit={(values) => {
              setCouriers((prev) => [
                ...prev,
                {
                  id: `c-${prev.length + 1}`,
                  ...values,
                  rating: Number(values.rating),
                  deliveries: Number(values.deliveries)
                }
              ]);
            }}
          />
          <RestaurantForm
            onSubmit={(values) => {
              setRestaurants((prev) => [
                ...prev,
                {
                  id: `r-${prev.length + 1}`,
                  ...values,
                  activeOrders: Number(values.activeOrders)
                }
              ]);
            }}
          />
        </div>
      </section>
    </div>
  );
}
