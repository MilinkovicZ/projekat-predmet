import React from 'react';
import {
  MapContainer,
  TileLayer
} from 'react-leaflet';
import 'leaflet/dist/leaflet.css';

const ProductsMap = () => {
  const position = [45.28039055604495, 19.82191364894804];

  return (
    <MapContainer
      center={position}
      zoom={10}
      style={{ width: "100vw", height: "100vh" }}
    >
      <TileLayer
        url="https://api.maptiler.com/maps/basic/256/{z}/{x}/{y}.png?key=rd3nk3oeO0Mp2SDfHI0X"
        attribution='<a href="https://www.maptiler.com/copyright/" target="_blank">&copy; MapTiler</a> <a href="https://www.openstreetmap.org/copyright" target="_blank">&copy; OpenStreetMap contributors</a>'
      />
    </MapContainer>
  );
};

export default ProductsMap;
