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

interface Department {
  id: number;
  name: string;
  description: string;
  doctorCount: number;
}

const emptyDepartment: Department = {
  id: 0,
  name: '',
  description: '',
  doctorCount: 0
};

const Departments: React.FC = () => {
  const [departments, setDepartments] = useState<Department[]>([]);
  const [department, setDepartment] = useState<Department>(emptyDepartment);
  const [openAddDialog, setOpenAddDialog] = useState(false);
  const [openEditDialog, setOpenEditDialog] = useState(false);
  const [openDeleteDialog, setOpenDeleteDialog] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');

  // Use local state for departments with localStorage for persistence
  const fetchDepartments = () => {
    // Try to get departments from localStorage first
    const storedDepartments = localStorage.getItem('hospitalAppDepartments');
    if (storedDepartments) {
      setDepartments(JSON.parse(storedDepartments));
    } else if (departments.length === 0) {
      // If no departments exist yet, set some sample data
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
  };

  useEffect(() => {
    fetchDepartments();
    
    // Set up event listener for localStorage changes
    const handleStorageChange = () => {
      const storedDepartments = localStorage.getItem('hospitalAppDepartments');
      if (storedDepartments) {
        setDepartments(JSON.parse(storedDepartments));
      }
    };
    
    // Check for updates every second (simple polling approach)
    const intervalId = setInterval(handleStorageChange, 1000);
    
    // Clean up interval on unmount
    return () => clearInterval(intervalId);
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    
    // For doctorCount field, convert to number
    if (name === 'doctorCount') {
      setDepartment(prev => ({ 
        ...prev, 
        [name]: value === '' ? 0 : parseInt(value, 10) 
      }));
    } else {
      setDepartment(prev => ({ ...prev, [name]: value }));
    }
  };

  const handleAdd = () => {
    // Add validation to ensure required fields are filled
    if (!department.name || !department.description) {
      alert('Please fill all required fields');
      return;
    }
    
    // Create a new department record with a unique ID
    const newDepartment = {
      ...department,
      id: departments.length > 0 ? Math.max(...departments.map(d => d.id)) + 1 : 1,
    };
    
    console.log('Adding new department record:', newDepartment);
    
    // Add to local state and localStorage
    const updatedDepartments = [...departments, newDepartment];
    setDepartments(updatedDepartments);
    localStorage.setItem('hospitalAppDepartments', JSON.stringify(updatedDepartments));
    
    // Close the dialog and reset the form
    setOpenAddDialog(false);
    setDepartment(emptyDepartment);
    alert('Department added successfully!');
  };

  const handleEdit = () => {
    // Update in local state and localStorage
    const updatedDepartments = departments.map(d => d.id === department.id ? department : d);
    setDepartments(updatedDepartments);
    localStorage.setItem('hospitalAppDepartments', JSON.stringify(updatedDepartments));
    
    setOpenEditDialog(false);
    setDepartment(emptyDepartment);
    alert('Department updated successfully!');
  };

  const handleDelete = () => {
    // Remove from local state and localStorage
    const updatedDepartments = departments.filter(d => d.id !== department.id);
    setDepartments(updatedDepartments);
    localStorage.setItem('hospitalAppDepartments', JSON.stringify(updatedDepartments));
    
    setOpenDeleteDialog(false);
    setDepartment(emptyDepartment);
    alert('Department deleted successfully!');
  };

  const openAdd = () => {
    setDepartment(emptyDepartment);
    setOpenAddDialog(true);
  };

  const openEdit = (department: Department) => {
    setDepartment(department);
    setOpenEditDialog(true);
  };

  const openDelete = (department: Department) => {
    setDepartment(department);
    setOpenDeleteDialog(true);
  };

  const filteredDepartments = departments.filter(department => 
    department.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
    department.description.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div style={{ padding: '20px' }}>
      <h1>Department Management</h1>
      
      <div style={{ display: 'flex', gap: '10px', marginBottom: '20px' }}>
        <div style={{ flexGrow: 1 }}>
          <TextField 
            label="Search departments..."
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
              <TableCell>Name</TableCell>
              <TableCell>Description</TableCell>
              <TableCell>DoctorCount</TableCell>
              <TableCell>Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {filteredDepartments.map((department) => (
              <TableRow key={department.id}>
                <TableCell>{department.name}</TableCell>
                <TableCell>{department.description}</TableCell>
                <TableCell>{department.doctorCount}</TableCell>
                <TableCell>
                  <Button onClick={() => openEdit(department)}>Edit</Button>
                  <Button onClick={() => openDelete(department)} color="error">Delete</Button>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      {/* Add Department Dialog */}
      <Dialog open={openAddDialog} onClose={() => setOpenAddDialog(false)}>
        <DialogTitle>Add New Department</DialogTitle>
        <DialogContent>
          <TextField
            margin="dense"
            name="name"
            label="Name"
            fullWidth
            value={department.name}
            onChange={handleInputChange}
          />
          <TextField
            margin="dense"
            name="description"
            label="Description"
            fullWidth
            value={department.description}
            onChange={handleInputChange}
          />
          <TextField
            margin="dense"
            name="doctorCount"
            label="Doctor Count"
            type="number"
            fullWidth
            value={department.doctorCount}
            onChange={handleInputChange}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenAddDialog(false)}>Cancel</Button>
          <Button onClick={handleAdd} color="primary">Add</Button>
        </DialogActions>
      </Dialog>

      {/* Edit Department Dialog */}
      <Dialog open={openEditDialog} onClose={() => setOpenEditDialog(false)}>
        <DialogTitle>Edit Department</DialogTitle>
        <DialogContent>
          <TextField
            margin="dense"
            name="name"
            label="Name"
            fullWidth
            value={department.name}
            onChange={handleInputChange}
          />
          <TextField
            margin="dense"
            name="description"
            label="Description"
            fullWidth
            value={department.description}
            onChange={handleInputChange}
          />
          <TextField
            margin="dense"
            name="doctorCount"
            label="Doctor Count"
            type="number"
            fullWidth
            value={department.doctorCount}
            onChange={handleInputChange}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenEditDialog(false)}>Cancel</Button>
          <Button onClick={handleEdit} color="primary">Save</Button>
        </DialogActions>
      </Dialog>

      {/* Delete Department Dialog */}
      <Dialog open={openDeleteDialog} onClose={() => setOpenDeleteDialog(false)}>
        <DialogTitle>Delete Department</DialogTitle>
        <DialogContent>
          Are you sure you want to delete the {department.name} department?
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenDeleteDialog(false)}>Cancel</Button>
          <Button onClick={handleDelete} color="error">Delete</Button>
        </DialogActions>
      </Dialog>
    </div>
  );
};

export default Departments;
