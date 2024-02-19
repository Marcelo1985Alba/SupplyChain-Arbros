// In development, always fetch from the network and do not enable offline support.
// This is because caching would make development more difficult (changes would not
// be reflected on the first load after each change).
var cacheNameVersion = 'scPWA-v0.81.25';
self.addEventListener('fetch', () => { });
