self.addEventListener('install', () => self.skipWaiting())
self.addEventListener('activate', (e) => e.waitUntil(self.clients.claim()))

// Online-only app: never cache. Forward every request straight to the network.
self.addEventListener('fetch', (event) => {
  event.respondWith(fetch(event.request))
})
