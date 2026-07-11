import path from 'path'
import { fileURLToPath } from 'url'
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

const __dirname = path.dirname(fileURLToPath(import.meta.url))

const apiHttps = process.env['services__resumeanalyzer-api__https__0']
const apiHttp = process.env['services__resumeanalyzer-api__http__0']
const apiTarget = apiHttps || apiHttp || 'https://localhost:5222'

export default defineConfig({
  base: '/resume-analyzer/',
  plugins: [
    react(),
    tailwindcss(),
  ],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
    },
  },
  server: {
    host: true,
    proxy: {
      '/api': {
        target: apiTarget,
        secure: false,
      },
    },
  },
})
