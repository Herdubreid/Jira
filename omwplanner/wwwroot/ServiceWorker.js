const baseURL = '/';
const indexURL = '/index.html';
const networkFetchEvent = 'fetch';
const swInstallEvent = 'install';
const swInstalledEvent = 'installed';
const swActivateEvent = 'activate';
const staticCachePrefix = 'blazor-cache-v';
const staticCacheName = 'blazor-cache-v7';
const requiredFiles = [
    "/_framework/blazor.server.js",
    "/favicon.ico",
    "/fontawsome/css/all.css",
    "/fontawsome/webfonts/fa-brands-400.eot",
    "/fontawsome/webfonts/fa-brands-400.svg",
    "/fontawsome/webfonts/fa-brands-400.ttf",
    "/fontawsome/webfonts/fa-brands-400.woff",
    "/fontawsome/webfonts/fa-brands-400.woff2",
    "/fontawsome/webfonts/fa-regular-400.eot",
    "/fontawsome/webfonts/fa-regular-400.svg",
    "/fontawsome/webfonts/fa-regular-400.ttf",
    "/fontawsome/webfonts/fa-regular-400.woff",
    "/fontawsome/webfonts/fa-regular-400.woff2",
    "/fontawsome/webfonts/fa-solid-900.eot",
    "/fontawsome/webfonts/fa-solid-900.svg",
    "/fontawsome/webfonts/fa-solid-900.ttf",
    "/fontawsome/webfonts/fa-solid-900.woff",
    "/fontawsome/webfonts/fa-solid-900.woff2",
    "/icon-192.png",
    "/icon-512.png",
    "/index.html",
    "/main.js",
    "/ServiceWorkerRegister.js",
    "/manifest.json",
    "/Style.min.css"
];
// * listen for the install event and pre-cache anything in filesToCache * //
self.addEventListener(swInstallEvent, event => {
    self.skipWaiting();
    event.waitUntil(
        caches.open(staticCacheName)
            .then(cache => {
                return cache.addAll(requiredFiles);
            })
    );
});
self.addEventListener(swActivateEvent, function (event) {
    event.waitUntil(
        caches.keys().then(function (cacheNames) {
            return Promise.all(
                cacheNames.map(function (cacheName) {
                    if (staticCacheName !== cacheName && cacheName.startsWith(staticCachePrefix)) {
                        return caches.delete(cacheName);
                    }
                })
            );
        })
    );
});
self.addEventListener(networkFetchEvent, event => {
    const requestUrl = new URL(event.request.url);
    if (requestUrl.origin === location.origin) {
        if (requestUrl.pathname === baseURL) {
            event.respondWith(caches.match(indexURL));
            return;
        }
    }
    event.respondWith(
        caches.match(event.request)
            .then(response => {
                if (response) {
                    return response;
                }
                return fetch(event.request)
                    .then(response => {
                        if (response.ok) {
                            if (requestUrl.origin === location.origin) {
                                caches.open(staticCacheName).then(cache => {
                                    cache.put(event.request.url, response);
                                });
                            }
                        }
                        return response.clone();
                    });
            }).catch(error => {
                console.error(error);
            })
    );
});
