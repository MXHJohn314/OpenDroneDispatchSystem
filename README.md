# FlyingPizza — Open Drone Dispatch System

An open-source drone fleet management system for small delivery operations. Dispatch, track, and monitor drones in real time from a browser.

---

## Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (includes Compose)
- [Node.js 20+](https://nodejs.org/) (for local frontend dev only)
- A [Google Maps API key](https://developers.google.com/maps/documentation/javascript/get-api-key) with the **Maps JavaScript API** and **Geocoding API** enabled

---

## Architecture

```
Browser (port 81)
    │
    ▼
FrontEnd-React   ──── DatabaseAccess (port 85) ──── MongoDB (port 27017)
                 ──── Dispatch (port 83)
                            │
                            ▼
                       SimDrone (port 84)
```

---

## Quick start

### 1. Clone the repo

```bash
git clone https://github.com/MXHJohn314/OpenDroneDispatchSystem.git
cd OpenDroneDispatchSystem
```

### 2. Set your Google Maps key

```bash
cp FrontEnd-React/.env.example FrontEnd-React/.env.local
# Edit FrontEnd-React/.env.local and set:
# VITE_GOOGLE_MAPS_API_KEY=<your key>
```

### 3. Start all services

```bash
docker compose up --build
```

This starts MongoDB, DatabaseAccess, Dispatch, SimDrone, and the React frontend. First build takes 3–5 minutes.

### 4. Seed the settings document (required before first use)

The services read runtime configuration (home location, API key, service URLs) from a MongoDB document. Insert it once:

```bash
docker exec -it opendronedeliverysystem-mongodb-1 mongosh
```

Inside the Mongo shell:

```js
use odds

db.Settings.insertOne({
  _id: "settings",
  DISPATCH_URL: "http://dispatch:80",
  DATABASE_ACCESS_URL: "http://database-access:80",
  DATABASE_NAME: "odds",
  FLEET_COLLECTION_NAME: "fleet",
  ORDERS_COLLECTION_NAME: "orders",
  CONNECTION_STRING: "mongodb://mongodb:27017",
  HOME_LOCATION: { Latitude: 39.7392, Longitude: -104.9903 },
  API_KEY: "<your Google Maps key>"
})

exit
```

> `HOME_LOCATION` is the drone base station. The example above uses downtown Denver — swap in any lat/lng you want.

### 5. Restart services so they pick up the settings

```bash
docker compose restart database-access dispatch simdrone
```

### 6. Open the app

Navigate to **http://localhost:81**

---

## Running a demo

### Step 1 — Add a drone

1. Go to the **Orders** page
2. In the Drone Manager panel, enter the SimDrone URL: `http://localhost:84`
3. Click **Add Drone**

The drone appears in the list with state `Unititialized`. The Dispatch service initializes it automatically within a few seconds and it moves to `Ready`.

### Step 2 — Create an order

1. Still on the **Orders** page, fill in a customer name and a real street address (geocoding requires a valid address)
2. Click **Create Order**

The order appears with state `Waiting`.

### Step 3 — Watch dispatch

Within 3 seconds the Dispatch background service pairs the waiting order with the ready drone. Both flip to `Assigned`, then the drone starts `Delivering`.

### Step 4 — Track the flight

1. Go to the **Tracking** page
2. The drone appears on the map as a colored triangle pointing in its direction of travel
3. A dashed line shows the flight path to the destination circle
4. The map refreshes every 2 seconds

When the drone reaches the delivery address it completes the order, returns home, and goes back to `Ready`.

---

## Local development (without Docker)

Run each service individually if you want faster iteration.

**MongoDB**
```bash
docker run -d -p 27017:27017 --name odds-mongo mongo:7
```

**DatabaseAccess** (port 85)
```bash
cd DatabaseAccess
CONNECTION_STRING=mongodb://localhost:27017 DATABASE_NAME=odds ODDS_SETTINGS=settings dotnet run
```

**Dispatch** (port 83)
```bash
cd Dispatch
DATABASE_ACCESS_URL=http://localhost:85 dotnet run
```

**SimDrone** (port 84)
```bash
cd SimDrone
DISPATCH_URL=http://localhost:83 dotnet run
```

**Frontend** (port 5173 in dev, proxies to backends automatically)
```bash
cd FrontEnd-React
npm install
npm run dev
```

Open **http://localhost:5173** — hot reload is enabled.

---

## Running tests

**C# backend**
```bash
dotnet test Tests/
```

**React frontend**
```bash
cd FrontEnd-React
npm run test
```

---

## Environment variables reference

| Service | Variable | Default in Compose | Description |
|---|---|---|---|
| DatabaseAccess | `CONNECTION_STRING` | `mongodb://mongodb:27017` | MongoDB URI |
| DatabaseAccess | `DATABASE_NAME` | `odds` | Database name |
| DatabaseAccess | `ODDS_SETTINGS` | `settings` | `_id` of the settings document |
| Dispatch | `DATABASE_ACCESS_URL` | `http://database-access:80` | DatabaseAccess base URL |
| SimDrone | `DISPATCH_URL` | `http://dispatch:80` | Dispatch base URL |
| FrontEnd-React | `VITE_GOOGLE_MAPS_API_KEY` | _(none)_ | Maps JS + Geocoding API key |

---

## Adding a real Tello drone

SimDrone includes a `TelloDrone` implementation for physical [Ryze Tello](https://www.ryzerobotics.com/tello) drones. Connect the Tello to Wi-Fi, then add its IP as the drone URL in the Orders page (e.g. `http://192.168.10.1`). The dispatch system handles it identically to a simulated drone.
