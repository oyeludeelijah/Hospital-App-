import { useEffect } from "react";
import { api } from "../services/api";

export default function TestAPI() {
  useEffect(() => {
    api.get("/patients").then(res => console.log("API Response:", res.data));
  }, []);
  
  return <div>Testing API...</div>;
}
