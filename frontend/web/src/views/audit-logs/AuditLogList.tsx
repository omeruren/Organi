'use client'

// React Imports
import { useState } from 'react'

// MUI Imports
import Card from '@mui/material/Card'
import CardHeader from '@mui/material/CardHeader'
import CardContent from '@mui/material/CardContent'
import Chip from '@mui/material/Chip'
import Typography from '@mui/material/Typography'
import TextField from '@mui/material/TextField'
import MenuItem from '@mui/material/MenuItem'
import Grid from '@mui/material/Grid'
import { DataGrid, GridActionsCellItem } from '@mui/x-data-grid'
import type { GridColDef, GridPaginationModel, GridRowParams } from '@mui/x-data-grid'

// Hook Imports
import { useAuditLogs } from '@/hooks/api/useAuditLogs'

// View Imports
import AuditLogDetailDialog from '@views/audit-logs/AuditLogDetailDialog'
import { auditActionColorMap, AUDIT_ENTITY_OPTIONS } from '@views/audit-logs/auditAction'

// Type Imports
import type { AuditLogResponse } from '@/types/api/auditLog'

const AuditLogList = () => {
  // States
  const [paginationModel, setPaginationModel] = useState<GridPaginationModel>({ page: 0, pageSize: 10 })
  const [entityName, setEntityName] = useState('')
  const [fromDate, setFromDate] = useState('')
  const [toDate, setToDate] = useState('')
  const [selectedLog, setSelectedLog] = useState<AuditLogResponse | null>(null)

  const { data, isLoading } = useAuditLogs({
    page: paginationModel.page + 1, // backend is 1-indexed, DataGrid is 0-indexed
    pageSize: paginationModel.pageSize,
    entityName: entityName || undefined,
    fromDate: fromDate || undefined,
    toDate: toDate || undefined
  })

  const resetToFirstPage = () => setPaginationModel(prev => ({ ...prev, page: 0 }))

  const columns: GridColDef<AuditLogResponse>[] = [
    {
      field: 'timestamp',
      headerName: 'When',
      width: 180,
      sortable: false,
      renderCell: params => <Typography variant='body2'>{new Date(params.row.timestamp).toLocaleString()}</Typography>
    },
    { field: 'entityName', headerName: 'Entity', width: 120, sortable: false },
    {
      field: 'action',
      headerName: 'Action',
      width: 150,
      sortable: false,
      renderCell: params => (
        <Chip label={params.value} color={auditActionColorMap[params.row.action]} size='small' variant='tonal' />
      )
    },
    {
      field: 'userEmail',
      headerName: 'User',
      flex: 1,
      minWidth: 180,
      sortable: false,
      renderCell: params => <Typography variant='body2'>{params.row.userEmail ?? '—'}</Typography>
    },
    {
      field: 'ipAddress',
      headerName: 'IP',
      width: 140,
      sortable: false,
      renderCell: params => <Typography variant='body2'>{params.row.ipAddress ?? '—'}</Typography>
    },
    {
      field: 'actions',
      type: 'actions',
      headerName: '',
      width: 70,
      getActions: (params: GridRowParams<AuditLogResponse>) => [
        <GridActionsCellItem
          key='view'
          icon={<i className='ri-eye-line' />}
          label='View'
          onClick={() => setSelectedLog(params.row)}
        />
      ]
    }
  ]

  return (
    <Card>
      <CardHeader title='Audit Logs' />
      <CardContent>
        <Grid container spacing={4}>
          <Grid item xs={12} sm={3}>
            <TextField
              select
              fullWidth
              size='small'
              label='Entity'
              value={entityName}
              onChange={e => {
                setEntityName(e.target.value)
                resetToFirstPage()
              }}
            >
              <MenuItem value=''>All Entities</MenuItem>
              {AUDIT_ENTITY_OPTIONS.map(option => (
                <MenuItem key={option} value={option}>
                  {option}
                </MenuItem>
              ))}
            </TextField>
          </Grid>
          <Grid item xs={12} sm={3}>
            <TextField
              fullWidth
              size='small'
              type='date'
              label='From'
              InputLabelProps={{ shrink: true }}
              value={fromDate}
              onChange={e => {
                setFromDate(e.target.value)
                resetToFirstPage()
              }}
            />
          </Grid>
          <Grid item xs={12} sm={3}>
            <TextField
              fullWidth
              size='small'
              type='date'
              label='To'
              InputLabelProps={{ shrink: true }}
              value={toDate}
              onChange={e => {
                setToDate(e.target.value)
                resetToFirstPage()
              }}
            />
          </Grid>
        </Grid>
      </CardContent>
      <DataGrid
        rows={data?.items ?? []}
        columns={columns}
        loading={isLoading}
        autoHeight
        paginationMode='server'
        rowCount={data?.totalCount ?? 0}
        paginationModel={paginationModel}
        onPaginationModelChange={setPaginationModel}
        pageSizeOptions={[10, 25, 50]}
        disableRowSelectionOnClick
        disableColumnMenu
      />
      <AuditLogDetailDialog log={selectedLog} onClose={() => setSelectedLog(null)} />
    </Card>
  )
}

export default AuditLogList
