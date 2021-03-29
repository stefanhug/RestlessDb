insert into [SYSADM].[TSY_QUERYREPOSITORY]([NAME], [LABEL], [DESCRIPTION], [SQL])
values('exammodes', 'Exam modes', 'Exam modes of CCW', 
	'select CODE, CODE_ENGLISH, BEZEICHNUNG from sysadm.TSY_BEREICH order by code')

insert into [SYSADM].[TSY_QUERYREPOSITORY]([NAME], [LABEL], [DESCRIPTION], [SQL])
values('workflowaudit', 'Workflow audit', 'Workflow audit table', 
	'select * from sysadm.WorkflowAudit order by id')

insert into [SYSADM].[TSY_QUERYREPOSITORY]([NAME], [LABEL], [DESCRIPTION], [SQL])
values('workflowauditdaterange', 'Workflow audit date range', 'Workflow audit table', 
	'select * from sysadm.WorkflowAudit where LogDateTime > @DATEFROM and LogDateTime <@DATETO order by id')


