import { api } from './api';

export interface Patient {
  id: number;
  firstName: string;
  lastName: string;
  dateOfBirth: string;
  gender: string;
  contactNumber: string;
  email: string;
  address: string;
}

export const getPatients = async (): Promise<Patient[]> => {
  try {
    const response = await api.get('/patients');
    return response.data;
  } catch (error) {
    console.error('Error fetching patients:', error);
    throw error;
  }
};

export const addPatient = async (patient: Omit<Patient, 'id'>): Promise<Patient> => {
  try {
    const response = await api.post('/patients', patient);
    return response.data;
  } catch (error) {
    console.error('Error adding patient:', error);
    throw error;
  }
};

export const updatePatient = async (id: number, patient: Patient): Promise<Patient> => {
  try {
    const response = await api.put(`/patients/${id}`, patient);
    return response.data;
  } catch (error) {
    console.error('Error updating patient:', error);
    throw error;
  }
};

export const deletePatient = async (id: number): Promise<void> => {
  try {
    await api.delete(`/patients/${id}`);
  } catch (error) {
    console.error('Error deleting patient:', error);
    throw error;
  }
}; 