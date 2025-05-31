import React, { useState, useEffect } from 'react';
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
import { Patient, getPatients, addPatient, updatePatient, deletePatient } from '../../services/patientService';

const emptyPatient: Patient = {
  id: 0,
  firstName: '',
  lastName: '',
  dateOfBirth: '',
  gender: '',
  contactNumber: '',
  email: '',
  address: ''
};

const Patients: React.FC = () => {
  const [patients, setPatients] = useState<Patient[]>([]);
  const [patient, setPatient] = useState<Patient>(emptyPatient);
  const [openAddDialog, setOpenAddDialog] = useState(false);
  const [openEditDialog, setOpenEditDialog] = useState(false);
  const [openDeleteDialog, setOpenDeleteDialog] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [error, setError] = useState<string>('');

  const fetchPatients = async () => {
    try {
      const data = await getPatients();
      setPatients(data);
      setError('');
    } catch (error) {
      setError('Error fetching patients. Please try again.');
      console.error('Error fetching patients:', error);
    }
  };

  useEffect(() => {
    fetchPatients();
  }, []);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setPatient(prev => ({ ...prev, [name]: value }));
  };

  const handleAdd = async () => {
    try {
      const { id, ...patientData } = patient;
      await addPatient(patientData);
      await fetchPatients();
      setOpenAddDialog(false);
      setPatient(emptyPatient);
      setError('');
    } catch (error) {
      setError('Error adding patient. Please try again.');
      console.error('Error adding patient:', error);
    }
  };

  const handleEdit = async () => {
    try {
      await updatePatient(patient.id, patient);
      await fetchPatients();
      setOpenEditDialog(false);
      setPatient(emptyPatient);
      setError('');
    } catch (error) {
      setError('Error updating patient. Please try again.');
      console.error('Error updating patient:', error);
    }
  };

  const handleDelete = async () => {
    try {
      await deletePatient(patient.id);
      await fetchPatients();
      setOpenDeleteDialog(false);
      setPatient(emptyPatient);
      setError('');
    } catch (error) {
      setError('Error deleting patient. Please try again.');
      console.error('Error deleting patient:', error);
    }
  };

  const openAdd = () => {
    setPatient(emptyPatient);
    setOpenAddDialog(true);
  };

  const openEdit = (patient: Patient) => {
    setPatient(patient);
    setOpenEditDialog(true);
  };

  const openDelete = (patient: Patient) => {
    setPatient(patient);
    setOpenDeleteDialog(true);
  };

  const filteredPatients = patients.filter(patient => 
    `${patient.firstName} ${patient.lastName}`.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div style={{ padding: '20px' }}>
      <h1>Patient Management</h1>
      
      {error && (
        <div style={{ 
          color: 'red', 
          backgroundColor: '#ffebee', 
          padding: '10px', 
          borderRadius: '4px',
          marginBottom: '20px' 
        }}>
          {error}
        </div>
      )}

      <div style={{ display: 'flex', gap: '10px', marginBottom: '20px' }}>
        <div style={{ flexGrow: 1 }}>
          <TextField 
            label="Search by name..."
            variant="outlined"
            fullWidth
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
          />
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
              <TableCell>Full Name</TableCell>
              <TableCell>Date of Birth</TableCell>
              <TableCell>Gender</TableCell>
              <TableCell>Contact Number</TableCell>
              <TableCell>Email</TableCell>
              <TableCell>Address</TableCell>
              <TableCell>Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {filteredPatients.map((patient) => (
              <TableRow key={patient.id}>
                <TableCell>{`${patient.firstName} ${patient.lastName}`}</TableCell>
                <TableCell>{patient.dateOfBirth ? format(new Date(patient.dateOfBirth), 'MM/dd/yyyy') : ''}</TableCell>
                <TableCell>{patient.gender}</TableCell>
                <TableCell>{patient.contactNumber}</TableCell>
                <TableCell>{patient.email}</TableCell>
                <TableCell>{patient.address}</TableCell>
                <TableCell>
                  <Button onClick={() => openEdit(patient)}>Edit</Button>
                  <Button onClick={() => openDelete(patient)} color="error">Delete</Button>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      {/* Add Patient Dialog */}
      <Dialog open={openAddDialog} onClose={() => setOpenAddDialog(false)}>
        <DialogTitle>Add New Patient</DialogTitle>
        <DialogContent>
          <TextField
            margin="dense"
            name="firstName"
            label="First Name"
            fullWidth
            value={patient.firstName}
            onChange={handleInputChange}
          />
          <TextField
            margin="dense"
            name="lastName"
            label="Last Name"
            fullWidth
            value={patient.lastName}
            onChange={handleInputChange}
          />
          <TextField
            margin="dense"
            name="dateOfBirth"
            label="Date of Birth"
            type="date"
            fullWidth
            InputLabelProps={{ shrink: true }}
            value={patient.dateOfBirth}
            onChange={handleInputChange}
          />
          <TextField
            margin="dense"
            name="gender"
            label="Gender"
            select
            fullWidth
            value={patient.gender}
            onChange={handleInputChange}
            SelectProps={{
              native: true,
            }}
          >
            <option value=""></option>
            <option value="Male">Male</option>
            <option value="Female">Female</option>
          </TextField>
          <TextField
            margin="dense"
            name="contactNumber"
            label="Contact Number"
            fullWidth
            value={patient.contactNumber}
            onChange={handleInputChange}
          />
          <TextField
            margin="dense"
            name="email"
            label="Email"
            fullWidth
            value={patient.email}
            onChange={handleInputChange}
          />
          <TextField
            margin="dense"
            name="address"
            label="Address"
            fullWidth
            value={patient.address}
            onChange={handleInputChange}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenAddDialog(false)}>Cancel</Button>
          <Button onClick={handleAdd} color="primary">Add</Button>
        </DialogActions>
      </Dialog>

      {/* Edit Patient Dialog */}
      <Dialog open={openEditDialog} onClose={() => setOpenEditDialog(false)}>
        <DialogTitle>Edit Patient</DialogTitle>
        <DialogContent>
          <TextField
            margin="dense"
            name="firstName"
            label="First Name"
            fullWidth
            value={patient.firstName}
            onChange={handleInputChange}
          />
          <TextField
            margin="dense"
            name="lastName"
            label="Last Name"
            fullWidth
            value={patient.lastName}
            onChange={handleInputChange}
          />
          <TextField
            margin="dense"
            name="dateOfBirth"
            label="Date of Birth"
            type="date"
            fullWidth
            InputLabelProps={{ shrink: true }}
            value={patient.dateOfBirth ? patient.dateOfBirth.split('T')[0] : ''}
            onChange={handleInputChange}
          />
          <TextField
            margin="dense"
            name="gender"
            label="Gender"
            select
            fullWidth
            value={patient.gender}
            onChange={handleInputChange}
            SelectProps={{
              native: true,
            }}
          >
            <option value=""></option>
            <option value="Male">Male</option>
            <option value="Female">Female</option>
          </TextField>
          <TextField
            margin="dense"
            name="contactNumber"
            label="Contact Number"
            fullWidth
            value={patient.contactNumber}
            onChange={handleInputChange}
          />
          <TextField
            margin="dense"
            name="email"
            label="Email"
            fullWidth
            value={patient.email}
            onChange={handleInputChange}
          />
          <TextField
            margin="dense"
            name="address"
            label="Address"
            fullWidth
            value={patient.address}
            onChange={handleInputChange}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenEditDialog(false)}>Cancel</Button>
          <Button onClick={handleEdit} color="primary">Save</Button>
        </DialogActions>
      </Dialog>

      {/* Delete Patient Dialog */}
      <Dialog open={openDeleteDialog} onClose={() => setOpenDeleteDialog(false)}>
        <DialogTitle>Delete Patient</DialogTitle>
        <DialogContent>
          Are you sure you want to delete {patient.firstName} {patient.lastName}?
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenDeleteDialog(false)}>Cancel</Button>
          <Button onClick={handleDelete} color="error">Delete</Button>
        </DialogActions>
      </Dialog>
    </div>
  );
};

export default Patients;
