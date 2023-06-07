import React from "react";
import { MapContainer, Marker, TileLayer, Popup } from "react-leaflet";
import "leaflet/dist/leaflet.css";
import { useState } from "react";
import sellerService from "../../services/sellerService";
import { useEffect } from "react";
import L from "leaflet";
import Geocode from "react-geocode";

const ProductsMap = () => {  
  Geocode.setApiKey(process.env.REACT_APP_GEOCODE_KEY);
  Geocode.setLanguage("en");
  Geocode.setRegion("rs");
  Geocode.setLocationType("ROOFTOP");
  Geocode.enableDebug();

  const position = [46.104586472419314, 19.664730942252326];
  const [markersPositions, setMarkersPositions] = useState([]);

  const markerIcon = new L.Icon({
    iconUrl: "package.png",
    iconSize: [40, 40],
    popupAnchor: [0, -15],
  });

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    const newOrders = await sellerService.getNewOrders();
    const markersData = [];

    for (const o of newOrders) {
      try {
        const response = await Geocode.fromAddress(o.deliveryAddress);
        const { lat, lng } = response.results[0].geometry.location;
        markersData.push({ id: o.id, lat:lat, lon:lng });
      } catch (error) {
        console.log(error);
      }
    }

    setMarkersPositions(markersData);
  };

  return (
    <MapContainer
      center={position}
      zoom={10}
      style={{ width: "100vw", height: "100vh" }}
    >
      <TileLayer url={process.env.REACT_APP_URL_KEY} />
      {markersPositions.map((marker) => (
        <div key={marker.id}>
          <Marker position={[marker.lat, marker.lon]} icon={markerIcon}>
            <Popup>Order number: {marker.id}</Popup>
          </Marker>
        </div>
      ))}
    </MapContainer>
  );
};

export default ProductsMap;
