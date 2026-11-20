import vue from '@vitejs/plugin-vue'
import { defineConfig } from 'vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [vue()],
  server: {
    port: 5173,
    proxy: {
      // ส่ง /api ไปที่ ASP.NET Core API (เลี่ยง CORS ใน dev — API ก็เปิด CORS ให้ 5173 ไว้เหมือนกัน)
      '/api': {
        target: 'http://localhost:5000',
        changeOrigin: true,
      },
    },
  },
})
