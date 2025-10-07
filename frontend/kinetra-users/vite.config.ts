import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react-swc';

export default defineConfig({
  plugins: [react()],
  server: {
    port: 4201,
    host: true
  },
  preview: {
    port: 4173,
    host: true
  }
});
