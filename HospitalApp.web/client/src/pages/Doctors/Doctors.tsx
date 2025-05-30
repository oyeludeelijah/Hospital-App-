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

interface Doctor {
  id: number;
  firstName: string;
  lastName: string;
  specialization: string;
  contactNumber: string;
  email: string;
  department: string;
}

const emptyDoctor: Doctor = {
  id: 0,
  firstName: '',
  lastName: '',
  specialization: '',
  contactNumber: '',
  email: '',
  department: ''
};

interface Department {
  id: number;
  name: string;
  description: string;
  doctorCount: number;
}

const Doctors: React.FC = () => {
  const [doctors, setDoctors] = useState<Doctor[]>([]);
  const [doctor, setDoctor] = useState<Doctor>(emptyDoctor);
  const [departments, setDepartments] = useState<Department[]>([]);
  const [openAddDialog, setOpenAddDialog] = useState(false);
  const [openEditDialog, setOpenEditDialog] = useState(false);
  const [openDeleteDialog, setOpenDeleteDialog] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');

  const fetchDoctors = () => {
    // Use local state instead of API calls for now
    if (doctors.length === 0) {
      setDoctors([
        {
          id: 1,
          firstName: 'John',
          lastName: 'Smith',
          specialization: 'Dermatology',
          contactNumber: '555-1234',
          email: 'john.smith@hospital.com',
          department: 'Cardiology'
        },
        {
          id: 2,
          firstName: 'Sarah',
          lastName: 'Johnson',
          specialization: 'Pediatrics',
          contactNumber: '555-5678',
          email: 'sarah.johnson@hospital.com',
          department: 'Pediatrics'
        }
      ]);
    }
  };

  const fetchDepartments = async () => {
    try {
      // Try to get departments from localStorage first
      const storedDepartments = localStorage.getItem('hospitalAppDepartments');
      if (storedDepartments) {
        setDepartments(JSON.parse(storedDepartments));
      } else {
        // Use default data if not found in localStorage
        const defaultDepartments = [
          {
            id: 1,
            name: 'Cardiology',
            description: 'Heart and cardiovascular system',
            doctorCount: 1
          },
          {
            id: 2,
            name: 'Pediatrics',
            description: 'Medical care for infants, children',
            doctorCount: 1
          },
          {
            id: 3,
            name: 'Orthopedics',
            description: 'Musculoskeletal system - bones',
            doctorCount: 0
          }
        ];
        setDepartments(defaultDepartments);
        localStorage.setItem('hospitalAppDepartments', JSON.stringify(defaultDepartments));
      }
    } catch (error) {
      console.error('Error fetching departments:', error);
    }
  };

  useEffect(() => {
    fetchDoctors();
    fetchDepartments();
  }, []);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    console.log(`Setting ${name} to ${value}`);
    setDoctor(prev => ({ ...prev, [name]: value }));
  };

  const handleAdd = () => {
    // Validate required fields
    if (!doctor.firstName || !doctor.lastName || !doctor.specialization) {
      alert('Please fill all required fields');
      return;
    }
    
    // Create a new doctor with a unique ID
    const newDoctor = {
      ...doctor,
      id: doctors.length > 0 ? Math.max(...doctors.map(d => d.id)) + 1 : 1
    };
    
    // Add the doctor to local state
    setDoctors([...doctors, newDoctor]);
    
    // Update the department's doctor count
    if (doctor.department) {
      // Find the department by name
      const departmentToUpdate = departments.find(d => d.name === doctor.department);
      
      if (departmentToUpdate) {
        // Update the doctor count locally
        const updatedDepartments = departments.map(d => {
          if (d.id === departmentToUpdate.id) {
            return { ...d, doctorCount: d.doctorCount + 1 };
          }
          return d;
        });
        
        setDepartments(updatedDepartments);
        // Save to localStorage for cross-component state sharing
        localStorage.setItem('hospitalAppDepartments', JSON.stringify(updatedDepartments));
      }
    }
    
    // Close the dialog and reset the form
    setOpenAddDialog(false);
    setDoctor(emptyDoctor);
    alert('Doctor added successfully!');
  };

  const handleEdit = () => {
    // Validate required fields
    if (!doctor.firstName || !doctor.lastName || !doctor.specialization) {
      alert('Please fill all required fields');
      return;
    }
    
    // Find the doctor before the edit to compare departments
    const originalDoctor = doctors.find(d => d.id === doctor.id);
    
    // If department changed, update the counts
    if (originalDoctor && originalDoctor.department !== doctor.department) {
      // Decrement count for old department
      if (originalDoctor.department) {
        const oldDepartment = departments.find(d => d.name === originalDoctor.department);
        if (oldDepartment && oldDepartment.doctorCount > 0) {
          const updatedDepartments = departments.map(d => {
            if (d.id === oldDepartment.id) {
              return { ...d, doctorCount: d.doctorCount - 1 };
            }
            return d;
          });
          setDepartments(updatedDepartments);
          // Save to localStorage for cross-component state sharing
          localStorage.setItem('hospitalAppDepartments', JSON.stringify(updatedDepartments));
        }
      }
      
      // Increment count for new department
      if (doctor.department) {
        const newDepartment = departments.find(d => d.name === doctor.department);
        if (newDepartment) {
          const updatedDepartments = departments.map(d => {
            if (d.id === newDepartment.id) {
              return { ...d, doctorCount: d.doctorCount + 1 };
            }
            return d;
          });
          setDepartments(updatedDepartments);
          // Save to localStorage for cross-component state sharing
          localStorage.setItem('hospitalAppDepartments', JSON.stringify(updatedDepartments));
        }
      }
    }
    
    // Update the doctor in local state
    const updatedDoctors = doctors.map(d => d.id === doctor.id ? doctor : d);
    setDoctors(updatedDoctors);
    
    // Close dialog and reset form
    setOpenEditDialog(false);
    setDoctor(emptyDoctor);
    alert('Doctor updated successfully!');
  };

  const handleDelete = () => {
    // Update the department's doctor count if the doctor had a department
    if (doctor.department) {
      // Find the department by name
      const departmentToUpdate = departments.find(d => d.name === doctor.department);
      
      if (departmentToUpdate && departmentToUpdate.doctorCount > 0) {
        // Update the doctor count locally
        const updatedDepartments = departments.map(d => {
          if (d.id === departmentToUpdate.id) {
            return { ...d, doctorCount: d.doctorCount - 1 };
          }
          return d;
        });
        
        setDepartments(updatedDepartments);
        // Save to localStorage for cross-component state sharing
        localStorage.setItem('hospitalAppDepartments', JSON.stringify(updatedDepartments));
      }
    }
    
    // Remove the doctor from local state
    const updatedDoctors = doctors.filter(d => d.id !== doctor.id);
    setDoctors(updatedDoctors);
    
    // Close dialog and reset form
    setOpenDeleteDialog(false);
    setDoctor(emptyDoctor);
    alert('Doctor deleted successfully!');
  };

  const openAdd = () => {
    setDoctor(emptyDoctor);
    setOpenAddDialog(true);
  };

  const openEdit = (doctor: Doctor) => {
    setDoctor(doctor);
    setOpenEditDialog(true);
  };

  const openDelete = (doctor: Doctor) => {
    setDoctor(doctor);
    setOpenDeleteDialog(true);
  };

  const filteredDoctors = doctors.filter(doctor => 
    `${doctor.firstName} ${doctor.lastName}`.toLowerCase().includes(searchTerm.toLowerCase()) ||
    doctor.specialization.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div style={{ padding: '20px' }}>
      <h1>Doctor Management</h1>
      
      <div style={{ display: 'flex', gap: '10px', marginBottom: '20px' }}>
        <div style={{ flexGrow: 1 }}>
          <TextField 
            label="Search by name or specialization..."
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
              <TableCell>Specialization</TableCell>
              <TableCell>Department</TableCell>
              <TableCell>Contact Number</TableCell>
              <TableCell>Email</TableCell>
              <TableCell>Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {filteredDoctors.map((doctor) => (
              <TableRow key={doctor.id}>
                <TableCell>{`${doctor.firstName} ${doctor.lastName}`}</TableCell>
                <TableCell>{doctor.specialization}</TableCell>
                <TableCell>{doctor.department}</TableCell>
                <TableCell>{doctor.contactNumber}</TableCell>
                <TableCell>{doctor.email}</TableCell>
                <TableCell>
                  <Button onClick={() => openEdit(doctor)}>Edit</Button>
                  <Button onClick={() => openDelete(doctor)} color="error">Delete</Button>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      {/* Add Doctor Dialog */}
      <Dialog open={openAddDialog} onClose={() => setOpenAddDialog(false)}>
        <DialogTitle>Add New Doctor</DialogTitle>
        <DialogContent>
          <TextField
            margin="dense"
            name="firstName"
            label="First Name"
            fullWidth
            value={doctor.firstName}
            onChange={handleInputChange}
          />
          <TextField
            margin="dense"
            name="lastName"
            label="Last Name"
            fullWidth
            value={doctor.lastName}
            onChange={handleInputChange}
          />
          <TextField
            margin="dense"
            name="specialization"
            label="Specialization"
            fullWidth
            select
            value={doctor.specialization}
            onChange={handleInputChange}
            SelectProps={{
              native: true,
            }}
          >
            <option value=""></option>
            <option value="Cardiology">Cardiology</option>
            <option value="Dermatology">Dermatology</option>
            <option value="Endocrinology">Endocrinology</option>
            <option value="Gastroenterology">Gastroenterology</option>
            <option value="Hematology">Hematology</option>
            <option value="Neurology">Neurology</option>
            <option value="Obstetrics">Obstetrics</option>
            <option value="Oncology">Oncology</option>
            <option value="Ophthalmology">Ophthalmology</option>
            <option value="Orthopedics">Orthopedics</option>
            <option value="Pediatrics">Pediatrics</option>
            <option value="Psychiatry">Psychiatry</option>
            <option value="Radiology">Radiology</option>
            <option value="Urology">Urology</option>
          </TextField>
          <TextField
            margin="dense"
            name="contactNumber"
            label="Contact Number"
            fullWidth
            value={doctor.contactNumber}
            onChange={handleInputChange}
          />
          <TextField
            margin="dense"
            name="email"
            label="Email"
            fullWidth
            value={doctor.email}
            onChange={handleInputChange}
          />
          <TextField
            margin="dense"
            name="department"
            label="Department"
            fullWidth
            select
            value={doctor.department}
            onChange={handleInputChange}
            SelectProps={{
              native: true,
            }}
          >
            <option value=""></option>
            {departments.map((dept) => (
              <option key={dept.id} value={dept.name}>{dept.name}</option>
            ))}
          </TextField>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenAddDialog(false)}>Cancel</Button>
          <Button onClick={handleAdd} color="primary">Add</Button>
        </DialogActions>
      </Dialog>

      {/* Edit Doctor Dialog */}
      <Dialog open={openEditDialog} onClose={() => setOpenEditDialog(false)}>
        <DialogTitle>Edit Doctor</DialogTitle>
        <DialogContent>
          <TextField
            margin="dense"
            name="firstName"
            label="First Name"
            fullWidth
            value={doctor.firstName}
            onChange={handleInputChange}
          />
          <TextField
            margin="dense"
            name="lastName"
            label="Last Name"
            fullWidth
            value={doctor.lastName}
            onChange={handleInputChange}
          />
          <TextField
            margin="dense"
            name="specialization"
            label="Specialization"
            fullWidth
            select
            value={doctor.specialization}
            onChange={handleInputChange}
            SelectProps={{
              native: true,
            }}
          >
            <option value=""></option>
            <option value="Cardiology">Cardiology</option>
            <option value="Dermatology">Dermatology</option>
            <option value="Endocrinology">Endocrinology</option>
            <option value="Gastroenterology">Gastroenterology</option>
            <option value="Hematology">Hematology</option>
            <option value="Neurology">Neurology</option>
            <option value="Obstetrics">Obstetrics</option>
            <option value="Oncology">Oncology</option>
            <option value="Ophthalmology">Ophthalmology</option>
            <option value="Orthopedics">Orthopedics</option>
            <option value="Pediatrics">Pediatrics</option>
            <option value="Psychiatry">Psychiatry</option>
            <option value="Radiology">Radiology</option>
            <option value="Urology">Urology</option>
          </TextField>
          <TextField
            margin="dense"
            name="contactNumber"
            label="Contact Number"
            fullWidth
            value={doctor.contactNumber}
            onChange={handleInputChange}
          />
          <TextField
            margin="dense"
            name="email"
            label="Email"
            fullWidth
            value={doctor.email}
            onChange={handleInputChange}
          />
          <TextField
            margin="dense"
            name="department"
            label="Department"
            fullWidth
            select
            value={doctor.department}
            onChange={handleInputChange}
            SelectProps={{
              native: true,
            }}
          >
            <option value=""></option>
            {departments.map((dept) => (
              <option key={dept.id} value={dept.name}>{dept.name}</option>
            ))}
          </TextField>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenEditDialog(false)}>Cancel</Button>
          <Button onClick={handleEdit} color="primary">Save</Button>
        </DialogActions>
      </Dialog>

      {/* Delete Doctor Dialog */}
      <Dialog open={openDeleteDialog} onClose={() => setOpenDeleteDialog(false)}>
        <DialogTitle>Delete Doctor</DialogTitle>
        <DialogContent>
          Are you sure you want to delete Dr. {doctor.firstName} {doctor.lastName}?
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenDeleteDialog(false)}>Cancel</Button>
          <Button onClick={handleDelete} color="error">Delete</Button>
        </DialogActions>
      </Dialog>
    </div>
  );
};

export default Doctors;
