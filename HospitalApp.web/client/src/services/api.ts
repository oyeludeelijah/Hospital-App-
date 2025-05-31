import axios from "axios";

export const api = axios.create({
  baseURL: "https://hospital-api-gbri.onrender.com/api",
  headers: { "Content-Type": "application/json" }
});
