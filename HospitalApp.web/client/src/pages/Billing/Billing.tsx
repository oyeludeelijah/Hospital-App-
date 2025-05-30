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

interface Billing {
  id: number;
  patientName: string;
  amount: number;
  billingDate: string;
  paymentStatus: string;
  paymentMethod: string;
}

const emptyBilling: Billing = {
  id: 0,
  patientName: '',
  amount: 0,
  billingDate: new Date().toISOString().split('T')[0],
  paymentStatus: '',
  paymentMethod: ''
};

const Billing: React.FC = () => {
  const [billings, setBillings] = useState<Billing[]>([]);
  const [billing, setBilling] = useState<Billing>(emptyBilling);
  const [openAddDialog, setOpenAddDialog] = useState(false);
  const [openEditDialog, setOpenEditDialog] = useState(false);
  const [openDeleteDialog, setOpenDeleteDialog] = useState(false);
  const [filterStatus, setFilterStatus] = useState<string>('All');
  const [patients, setPatients] = useState<{id: number, firstName: string, lastName: string}[]>([]);

  // Use local state for billings until backend is fixed
  const fetchBillings = () => {
    // If no billings exist yet, set some sample data
    if (billings.length === 0) {
      setBillings([
        {
          id: 1,
          patientName: 'Michael Brown',
          amount: 350.00,
          billingDate: '2025-05-15',
          paymentStatus: 'Paid',
          paymentMethod: 'Credit Card'
        },
        {
          id: 2,
          patientName: 'Emily Davis',
          amount: 500.00,
          billingDate: '2025-05-20',
          paymentStatus: 'Pending',
          paymentMethod: 'Insurance'
        }
      ]);
    }
  };

  const fetchPatients = async () => {
    try {
      const response = await axios.get('http://localhost:5005/api/patients');
      setPatients(response.data);
    } catch (error) {
      console.error('Error fetching patients:', error);
    }
  };

  useEffect(() => {
    fetchBillings();
    fetchPatients();
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    
    // For amount field, we'll keep it as string in the state but parse it when submitting
    setBilling(prev => ({ ...prev, [name]: value }));
  };

  const handleAdd = () => {
    // Add validation to ensure required fields are filled
    if (!billing.patientName || billing.amount <= 0 || !billing.billingDate || !billing.paymentStatus || !billing.paymentMethod) {
      alert('Please fill all required fields');
      return;
    }
    
    // Create a new billing record with a unique ID
    const newBilling = {
      ...billing,
      id: billings.length > 0 ? Math.max(...billings.map(b => b.id)) + 1 : 1,
      // Ensure amount is a number
      amount: typeof billing.amount === 'string' ? parseFloat(billing.amount) : billing.amount
    };
    
    console.log('Adding new billing record:', newBilling);
    
    // Add to local state directly (temporarily bypassing the API)
    setBillings([...billings, newBilling]);
    
    // Close the dialog and reset the form
    setOpenAddDialog(false);
    setBilling(emptyBilling);
    
    // Show success message
    alert('Billing added successfully!');
  };

  const handleEdit = () => {
    // Update in local state
    setBillings(billings.map(b => b.id === billing.id ? {
      ...billing,
      // Ensure amount is a number
      amount: typeof billing.amount === 'string' ? parseFloat(billing.amount) : billing.amount
    } : b));
    setOpenEditDialog(false);
    setBilling(emptyBilling);
    alert('Billing updated successfully!');
  };

  const handleDelete = () => {
    // Remove from local state
    setBillings(billings.filter(b => b.id !== billing.id));
    setOpenDeleteDialog(false);
    setBilling(emptyBilling);
    alert('Billing deleted successfully!');
  };

  const openAdd = () => {
    setBilling(emptyBilling);
    setOpenAddDialog(true);
  };

  const openEdit = (billing: Billing) => {
    setBilling({
      ...billing,
      billingDate: new Date(billing.billingDate).toISOString().split('T')[0]
    });
    setOpenEditDialog(true);
  };

  const openDelete = (billing: Billing) => {
    setBilling(billing);
    setOpenDeleteDialog(true);
  };

  const filteredBillings = billings.filter(billing => {
    if (filterStatus === 'All') return true;
    return billing.paymentStatus === filterStatus;
  });

  return (
    <div style={{ padding: '20px' }}>
      <h1>Billing Management</h1>
      
      <div style={{ display: 'flex', gap: '10px', marginBottom: '20px' }}>
        <div style={{ flexGrow: 1 }}>
          <TextField 
            select
            variant="outlined"
            fullWidth
            value={filterStatus}
            onChange={(e) => setFilterStatus(e.target.value)}
            SelectProps={{
              native: true,
            }}
          >
            <option value="All">All</option>
            <option value="Paid">Paid</option>
            <option value="Pending">Pending</option>
            <option value="Overdue">Overdue</option>
          </TextField>
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
              <TableCell>Amount</TableCell>
              <TableCell>Billing Date</TableCell>
              <TableCell>Payment Status</TableCell>
              <TableCell>Payment Method</TableCell>
              <TableCell>Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {filteredBillings.map((billing) => (
              <TableRow key={billing.id}>
                <TableCell>{billing.patientName}</TableCell>
                <TableCell>${billing.amount.toFixed(2)}</TableCell>
                <TableCell>{format(new Date(billing.billingDate), 'MM/dd/yyyy')}</TableCell>
                <TableCell>{billing.paymentStatus}</TableCell>
                <TableCell>{billing.paymentMethod}</TableCell>
                <TableCell>
                  <Button onClick={() => openEdit(billing)}>Edit</Button>
                  <Button onClick={() => openDelete(billing)} color="error">Delete</Button>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      {/* Add Billing Dialog */}
      <Dialog open={openAddDialog} onClose={() => setOpenAddDialog(false)}>
        <DialogTitle>Add New Billing</DialogTitle>
        <DialogContent>
          <TextField
            margin="dense"
            name="patientName"
            label="Patient Name"
            select
            fullWidth
            value={billing.patientName}
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
            name="amount"
            label="Amount"
            type="number"
            fullWidth
            value={billing.amount}
            onChange={handleInputChange}
          />
          <TextField
            margin="dense"
            name="billingDate"
            label="Billing Date"
            type="date"
            fullWidth
            InputLabelProps={{ shrink: true }}
            value={billing.billingDate}
            onChange={handleInputChange}
          />
          <TextField
            margin="dense"
            name="paymentStatus"
            label="Payment Status"
            select
            fullWidth
            value={billing.paymentStatus}
            onChange={handleInputChange}
            SelectProps={{
              native: true,
            }}
          >
            <option value=""></option>
            <option value="Paid">Paid</option>
            <option value="Pending">Pending</option>
            <option value="Overdue">Overdue</option>
          </TextField>
          <TextField
            margin="dense"
            name="paymentMethod"
            label="Payment Method"
            select
            fullWidth
            value={billing.paymentMethod}
            onChange={handleInputChange}
            SelectProps={{
              native: true,
            }}
          >
            <option value=""></option>
            <option value="Cash">Cash</option>
            <option value="Credit Card">Credit Card</option>
            <option value="Debit Card">Debit Card</option>
            <option value="Insurance">Insurance</option>
            <option value="Bank Transfer">Bank Transfer</option>
          </TextField>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenAddDialog(false)}>Cancel</Button>
          <Button onClick={handleAdd} color="primary">Add</Button>
        </DialogActions>
      </Dialog>

      {/* Edit Billing Dialog */}
      <Dialog open={openEditDialog} onClose={() => setOpenEditDialog(false)}>
        <DialogTitle>Edit Billing</DialogTitle>
        <DialogContent>
          <TextField
            margin="dense"
            name="patientName"
            label="Patient Name"
            select
            fullWidth
            value={billing.patientName}
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
            name="amount"
            label="Amount"
            type="number"
            fullWidth
            value={billing.amount}
            onChange={handleInputChange}
          />
          <TextField
            margin="dense"
            name="billingDate"
            label="Billing Date"
            type="date"
            fullWidth
            InputLabelProps={{ shrink: true }}
            value={billing.billingDate}
            onChange={handleInputChange}
          />
          <TextField
            margin="dense"
            name="paymentStatus"
            label="Payment Status"
            select
            fullWidth
            value={billing.paymentStatus}
            onChange={handleInputChange}
            SelectProps={{
              native: true,
            }}
          >
            <option value=""></option>
            <option value="Paid">Paid</option>
            <option value="Pending">Pending</option>
            <option value="Overdue">Overdue</option>
          </TextField>
          <TextField
            margin="dense"
            name="paymentMethod"
            label="Payment Method"
            select
            fullWidth
            value={billing.paymentMethod}
            onChange={handleInputChange}
            SelectProps={{
              native: true,
            }}
          >
            <option value=""></option>
            <option value="Cash">Cash</option>
            <option value="Credit Card">Credit Card</option>
            <option value="Debit Card">Debit Card</option>
            <option value="Insurance">Insurance</option>
            <option value="Bank Transfer">Bank Transfer</option>
          </TextField>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenEditDialog(false)}>Cancel</Button>
          <Button onClick={handleEdit} color="primary">Save</Button>
        </DialogActions>
      </Dialog>

      {/* Delete Billing Dialog */}
      <Dialog open={openDeleteDialog} onClose={() => setOpenDeleteDialog(false)}>
        <DialogTitle>Delete Billing</DialogTitle>
        <DialogContent>
          Are you sure you want to delete the billing record for {billing.patientName}?
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenDeleteDialog(false)}>Cancel</Button>
          <Button onClick={handleDelete} color="error">Delete</Button>
        </DialogActions>
      </Dialog>
    </div>
  );
};

export default Billing;
