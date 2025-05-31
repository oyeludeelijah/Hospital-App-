import React, { useState, useEffect } from 'react';
import axios from 'axios';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import Paper from '@mui/material/Paper';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import { format } from 'date-fns';
import { api } from '../../services/api';

interface Appointment {
  id: number;
  patientName: string;
  doctorName: string;
  appointmentDate: string;
  purpose: string;
  status: string;
}

const emptyAppointment: Appointment = {
  id: 0,
  patientName: '',
  doctorName: '',
  appointmentDate: new Date().toISOString().slice(0, 16),
  purpose: '',
  status: 'Scheduled'
};

const Appointments: React.FC = () => {
  const [appointments, setAppointments] = useState<Appointment[]>([]);
  const [appointment, setAppointment] = useState<Appointment>(emptyAppointment);
  const [openAddDialog, setOpenAddDialog] = useState(false);
  const [openEditDialog, setOpenEditDialog] = useState(false);
  const [openDeleteDialog, setOpenDeleteDialog] = useState(false);
  const [filterDate, setFilterDate] = useState<string>('');
  const [patients, setPatients] = useState<{id: number, firstName: string, lastName: string}[]>([]);
  const [doctors, setDoctors] = useState<{id: number, firstName: string, lastName: string, specialization: string}[]>([]);

  const fetchAppointments = async () => {
    try {
      const response = await api.get('/appointments');
      setAppointments(response.data);
    } catch (error) {
      console.error('Error fetching appointments:', error);
    }
  };

  const fetchPatients = async () => {
    try {
      const response = await api.get('/patients');
      setPatients(response.data);
    } catch (error) {
      console.error('Error fetching patients:', error);
    }
  };

  const fetchDoctors = async () => {
    try {
      const response = await api.get('/doctors');
      setDoctors(response.data);
    } catch (error) {
      console.error('Error fetching doctors:', error);
    }
  };

  useEffect(() => {
    fetchAppointments();
    fetchPatients();
    fetchDoctors();
  }, []);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setAppointment(prev => ({ ...prev, [name]: value }));
  };

  const handleAdd = async () => {
    try {
      await api.post('/appointments', appointment);
      fetchAppointments();
      setOpenAddDialog(false);
      setAppointment(emptyAppointment);
    } catch (error) {
      console.error('Error adding appointment:', error);
    }
  };

  const handleEdit = async () => {
    try {
      await api.put(`/appointments/${appointment.id}`, appointment);
      fetchAppointments();
      setOpenEditDialog(false);
      setAppointment(emptyAppointment);
    } catch (error) {
      console.error('Error updating appointment:', error);
    }
  };

  const handleDelete = async () => {
    try {
      await api.delete(`/appointments/${appointment.id}`);
      fetchAppointments();
      setOpenDeleteDialog(false);
      setAppointment(emptyAppointment);
    } catch (error) {
      console.error('Error deleting appointment:', error);
    }
  };

  const openAdd = () => {
    setAppointment(emptyAppointment);
    setOpenAddDialog(true);
  };

  const openEdit = (appointment: Appointment) => {
    setAppointment({
      ...appointment,
      appointmentDate: new Date(appointment.appointmentDate).toISOString().slice(0, 16)
    });
    setOpenEditDialog(true);
  };

  const openDelete = (appointment: Appointment) => {
    setAppointment(appointment);
    setOpenDeleteDialog(true);
  };

  const handleClearFilter = () => {
    setFilterDate('');
  };

  const filteredAppointments = appointments.filter(appointment => {
    if (!filterDate) return true;
    return appointment.appointmentDate.startsWith(filterDate);
  });

  return (
    <div style={{ padding: '20px' }}>
      <h1>Appointment Management</h1>
      
      <div style={{ display: 'flex', gap: '10px', marginBottom: '20px' }}>
        <div style={{ flexGrow: 1 }}>
          <TextField 
            type="date"
            variant="outlined"
            fullWidth
            value={filterDate}
            onChange={(e) => setFilterDate(e.target.value)}
            InputLabelProps={{ shrink: true }}
          />
        </div>
        <div>
          <Button variant="outlined" onClick={handleClearFilter}>
            Clear
          </Button>
        </div>
        <div>
          <Button variant="contained" color="primary" onClick={openAdd}>
            Add
          </Button>
        </div>
      </div>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Patient Name</TableCell>
              <TableCell>Doctor Name</TableCell>
              <TableCell>Appointment Date</TableCell>
              <TableCell>Purpose</TableCell>
              <TableCell>Status</TableCell>
              <TableCell>Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {filteredAppointments.map((appointment) => (
              <TableRow key={appointment.id}>
                <TableCell>{appointment.patientName}</TableCell>
                <TableCell>{appointment.doctorName}</TableCell>
                <TableCell>
                  {format(new Date(appointment.appointmentDate), 'MM/dd/yyyy HH:mm')}
                </TableCell>
                <TableCell>{appointment.purpose}</TableCell>
                <TableCell>{appointment.status}</TableCell>
                <TableCell>
                  <Button onClick={() => openEdit(appointment)}>Edit</Button>
                  <Button onClick={() => openDelete(appointment)} color="error">Delete</Button>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      {/* Add Appointment Dialog */}
      <Dialog open={openAddDialog} onClose={() => setOpenAddDialog(false)}>
        <DialogTitle>Add New Appointment</DialogTitle>
        <DialogContent>
          <TextField
            margin="dense"
            name="patientName"
            label="Patient Name"
            select
            fullWidth
            value={appointment.patientName}
            onChange={handleInputChange}
            SelectProps={{
              native: true,
            }}
          >
            <option value=""></option>
            {patients.map((patient) => (
              <option key={patient.id} value={`${patient.firstName} ${patient.lastName}`}>
                {patient.firstName} {patient.lastName}
              </option>
            ))}
          </TextField>
          <TextField
            margin="dense"
            name="doctorName"
            label="Doctor Name"
            select
            fullWidth
            value={appointment.doctorName}
            onChange={handleInputChange}
            SelectProps={{
              native: true,
            }}
          >
            <option value=""></option>
            {doctors.map((doctor) => (
              <option key={doctor.id} value={`Dr. ${doctor.firstName} ${doctor.lastName}`}>
                Dr. {doctor.firstName} {doctor.lastName} ({doctor.specialization})
              </option>
            ))}
          </TextField>
          <TextField
            margin="dense"
            name="appointmentDate"
            label="Appointment Date"
            type="datetime-local"
            fullWidth
            InputLabelProps={{ shrink: true }}
            value={appointment.appointmentDate}
            onChange={handleInputChange}
          />
          <TextField
            margin="dense"
            name="purpose"
            label="Purpose"
            fullWidth
            value={appointment.purpose}
            onChange={handleInputChange}
          />
          <TextField
            margin="dense"
            name="status"
            label="Status"
            select
            fullWidth
            value={appointment.status}
            onChange={handleInputChange}
            SelectProps={{
              native: true,
            }}
          >
            <option value="Scheduled">Scheduled</option>
            <option value="Completed">Completed</option>
            <option value="Cancelled">Cancelled</option>
          </TextField>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenAddDialog(false)}>Cancel</Button>
          <Button onClick={handleAdd} color="primary">Add</Button>
        </DialogActions>
      </Dialog>

      {/* Edit Appointment Dialog */}
      <Dialog open={openEditDialog} onClose={() => setOpenEditDialog(false)}>
        <DialogTitle>Edit Appointment</DialogTitle>
        <DialogContent>
          <TextField
            margin="dense"
            name="patientName"
            label="Patient Name"
            select
            fullWidth
            value={appointment.patientName}
            onChange={handleInputChange}
            SelectProps={{
              native: true,
            }}
          >
            <option value=""></option>
            {patients.map((patient) => (
              <option key={patient.id} value={`${patient.firstName} ${patient.lastName}`}>
                {patient.firstName} {patient.lastName}
              </option>
            ))}
          </TextField>
          <TextField
            margin="dense"
            name="doctorName"
            label="Doctor Name"
            select
            fullWidth
            value={appointment.doctorName}
            onChange={handleInputChange}
            SelectProps={{
              native: true,
            }}
          >
            <option value=""></option>
            {doctors.map((doctor) => (
              <option key={doctor.id} value={`Dr. ${doctor.firstName} ${doctor.lastName}`}>
                Dr. {doctor.firstName} {doctor.lastName} ({doctor.specialization})
              </option>
            ))}
          </TextField>
          <TextField
            margin="dense"
            name="appointmentDate"
            label="Appointment Date"
            type="datetime-local"
            fullWidth
            InputLabelProps={{ shrink: true }}
            value={appointment.appointmentDate}
            onChange={handleInputChange}
          />
          <TextField
            margin="dense"
            name="purpose"
            label="Purpose"
            fullWidth
            value={appointment.purpose}
            onChange={handleInputChange}
          />
          <TextField
            margin="dense"
            name="status"
            label="Status"
            select
            fullWidth
            value={appointment.status}
            onChange={handleInputChange}
            SelectProps={{
              native: true,
            }}
          >
            <option value="Scheduled">Scheduled</option>
            <option value="Completed">Completed</option>
            <option value="Cancelled">Cancelled</option>
          </TextField>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenEditDialog(false)}>Cancel</Button>
          <Button onClick={handleEdit} color="primary">Save</Button>
        </DialogActions>
      </Dialog>

      {/* Delete Appointment Dialog */}
      <Dialog open={openDeleteDialog} onClose={() => setOpenDeleteDialog(false)}>
        <DialogTitle>Delete Appointment</DialogTitle>
        <DialogContent>
          Are you sure you want to delete the appointment for {appointment.patientName} with {appointment.doctorName}?
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenDeleteDialog(false)}>Cancel</Button>
          <Button onClick={handleDelete} color="error">Delete</Button>
        </DialogActions>
      </Dialog>
    </div>
  );
};

export default Appointments;
