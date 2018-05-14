--*******************************************************************************************************************************
--****************GLOBAL REFERENCE DATA - APPLIES TO ALL CUSTOMERS  *************************************************************
--*******************************************************************************************************************************

--****************GENERAL APP SETTINGS  *****************************************************************************************
INSERT INTO T_OE_APP_SETTINGS ([SETTING_NAME],[SETTING_VALUE],[SETTING_DESC],[MODIFY_USERIDX],[MODIFY_DT]) VALUES ('EMAIL_FROM','info@eenterpriseinventorysandbox.org','The email address in the FROM line when sending emails from this application.',0,GetDate());
INSERT INTO T_OE_APP_SETTINGS ([SETTING_NAME],[SETTING_VALUE],[SETTING_DESC],[MODIFY_USERIDX],[MODIFY_DT]) VALUES ('EMAIL_SERVER','smtp.sendgrid.net','The SMTP email server used to allow this application to send emails.',0,GetDate());
INSERT INTO T_OE_APP_SETTINGS ([SETTING_NAME],[SETTING_VALUE],[SETTING_DESC],[MODIFY_USERIDX],[MODIFY_DT]) VALUES ('EMAIL_PORT','25','The port used to access the SMTP email server.',0,GetDate());
INSERT INTO T_OE_APP_SETTINGS ([SETTING_NAME],[SETTING_VALUE],[SETTING_DESC],[MODIFY_USERIDX],[MODIFY_DT]) VALUES ('EMAIL_SECURE_USER','smtp@change.me','If the SMTP server requires authentication, this is the SMTP server username.',0,GetDate());
INSERT INTO T_OE_APP_SETTINGS ([SETTING_NAME],[SETTING_VALUE],[SETTING_DESC],[ENCRYPT_IND],[MODIFY_USERIDX],[MODIFY_DT]) VALUES ('EMAIL_SECURE_PWD','change.me','If the SMTP server requires authentication, this is the SMTP server password or API KEY.',1,0,GetDate());
INSERT INTO T_OE_APP_SETTINGS ([SETTING_NAME],[SETTING_VALUE],[SETTING_DESC],[MODIFY_USERIDX],[MODIFY_DT]) VALUES ('PUBLIC_APP_PATH','http://www.eenterpriseinventorysandbox.org','The full URL of the deployed application. This is used when sending emails.',0,GetDate());
INSERT INTO T_OE_APP_SETTINGS ([SETTING_NAME],[SETTING_VALUE],[SETTING_DESC],[ENCRYPT_IND],[MODIFY_USERIDX],[MODIFY_DT]) VALUES ('AZURE_SEARCH_SVC_NAME','eenterprisepartnerprofile','The name of the Azure Search service.',0,0,GetDate());
INSERT INTO T_OE_APP_SETTINGS ([SETTING_NAME],[SETTING_VALUE],[SETTING_DESC],[ENCRYPT_IND],[MODIFY_USERIDX],[MODIFY_DT]) VALUES ('AZURE_SEARCH_ADMIN_KEY','change.me','The Admin Key for the Azure Search service.',1,0,GetDate());
INSERT INTO T_OE_APP_SETTINGS ([SETTING_NAME],[SETTING_VALUE],[SETTING_DESC],[ENCRYPT_IND],[MODIFY_USERIDX],[MODIFY_DT]) VALUES ('AZURE_SEARCH_QUERY_KEY','change.me','The Query Key for the Azure Search service.',1,0,GetDate());


--****************APP ROLES *****************************************************************************************
insert into T_OE_ROLES (ROLE_NAME, ROLE_DESC, CREATE_USERIDX, CREATE_DT) values ('Admins', 'Application Administrator can access administrative functions', 0, GetDate());


--****************APP USERS*****************************************************************************************
insert into T_OE_USERS (USER_ID, PWD_HASH, PWD_SALT, FNAME, LNAME, EMAIL, ACT_IND, INITAL_PWD_FLAG, EFFECTIVE_DT, LASTLOGIN_DT, PHONE, PHONE_EXT, CREATE_USERIDX, CREATE_DT, MODIFY_USERIDX, MODIFY_DT)
values ('system', 'notused','notused', 'system','','notused', 0, 0, GETDATE(), null, null, null, 0,GETDATE(),0,GETDATE());

insert into T_OE_USERS (USER_ID, PWD_HASH, PWD_SALT, FNAME, LNAME, EMAIL, ACT_IND, INITAL_PWD_FLAG, EFFECTIVE_DT, LASTLOGIN_DT, PHONE, PHONE_EXT, CREATE_USERIDX, CREATE_DT, MODIFY_USERIDX, MODIFY_DT)
values ('superadmin@change.me', 'pwd','', 'First','Last','superadmin@change.me', 1,1,GETDATE(),null, '555-555-5555', null, 0,GETDATE(),0,GETDATE());


--****************APP USER ROLES *****************************************************************************************
insert into T_OE_USER_ROLES (USER_IDX, ROLE_IDX) values (1,1);


--****************REF_STATE  *****************************************************************************************
INSERT into [T_OE_REF_STATE] values 
('AL', 'Alabama'),
('AK', 'Alaska'),
('AS', 'American Samoa'),
('AZ', 'Arizona'),
('AR', 'Arkansas'),
('CA', 'California'),
('CO', 'Colorado'),
('CT', 'Connecticut'),
('DE', 'Delaware'),
('DC', 'District of Columbia'),
('FL', 'Florida'),
('GA', 'Georgia'),
('GU', 'Guam'),
('HI', 'Hawaii'),
('ID', 'Idaho'),
('IL', 'Illinois'),
('IN', 'Indiana'),
('IA', 'Iowa'),
('KS', 'Kansas'),
('KY', 'Kentucky'),
('LA', 'Louisiana'),
('ME', 'Maine'),
('MD', 'Maryland'),
('MA', 'Massachusetts'),
('MI', 'Michigan'),
('MN', 'Minnesota'),
('MS', 'Mississippi'),
('MO', 'Missouri'),
('MT', 'Montana'),
('NE', 'Nebraska'),
('NV', 'Nevada'),
('NH', 'New Hampshire'),
('NJ', 'New Jersey'),
('NM', 'New Mexico'),
('NY', 'New York'),
('NC', 'North Carolina'),
('ND', 'North Dakota'),
('OH', 'Ohio'),
('OK', 'Oklahoma'),
('OR', 'Oregon'),
('PA', 'Pennsylvania'),
('PR', 'Puerto Rico'),
('RI', 'Rhode Island'),
('SC', 'South Carolina'),
('SD', 'South Dakota'),
('TN', 'Tennessee'),
('TX', 'Texas'),
('UT', 'Utah'),
('VT', 'Vermont'),
('VA', 'Virginia'),
('WA', 'Washington'),
('WV', 'West Virginia'),
('WI', 'Wisconsin'),
('WY', 'Wyoming');

update T_OE_REF_STATE set POP_DENSITY = 'High' where STATE_CD = 'NJ'
update T_OE_REF_STATE set POP_DENSITY = 'High' where STATE_CD = 'RI'
update T_OE_REF_STATE set POP_DENSITY = 'High' where STATE_CD = 'MA'
update T_OE_REF_STATE set POP_DENSITY = 'High' where STATE_CD = 'CT'
update T_OE_REF_STATE set POP_DENSITY = 'High' where STATE_CD = 'MD'
update T_OE_REF_STATE set POP_DENSITY = 'High' where STATE_CD = 'DE'
update T_OE_REF_STATE set POP_DENSITY = 'High' where STATE_CD = 'NY'
update T_OE_REF_STATE set POP_DENSITY = 'High' where STATE_CD = 'FL'
update T_OE_REF_STATE set POP_DENSITY = 'Medium' where STATE_CD = 'PA'
update T_OE_REF_STATE set POP_DENSITY = 'Medium' where STATE_CD = 'OH'
update T_OE_REF_STATE set POP_DENSITY = 'Medium' where STATE_CD = 'CA'
update T_OE_REF_STATE set POP_DENSITY = 'Medium' where STATE_CD = 'IL'
update T_OE_REF_STATE set POP_DENSITY = 'Medium' where STATE_CD = 'HI'
update T_OE_REF_STATE set POP_DENSITY = 'Medium' where STATE_CD = 'VA'
update T_OE_REF_STATE set POP_DENSITY = 'Medium' where STATE_CD = 'NC'
update T_OE_REF_STATE set POP_DENSITY = 'Medium' where STATE_CD = 'IN'
update T_OE_REF_STATE set POP_DENSITY = 'Medium' where STATE_CD = 'GA'
update T_OE_REF_STATE set POP_DENSITY = 'Medium' where STATE_CD = 'MI'
update T_OE_REF_STATE set POP_DENSITY = 'Medium' where STATE_CD = 'SC'
update T_OE_REF_STATE set POP_DENSITY = 'Medium' where STATE_CD = 'TN'
update T_OE_REF_STATE set POP_DENSITY = 'Medium' where STATE_CD = 'NH'
update T_OE_REF_STATE set POP_DENSITY = 'Medium' where STATE_CD = 'KY'
update T_OE_REF_STATE set POP_DENSITY = 'Medium' where STATE_CD = 'LA'
update T_OE_REF_STATE set POP_DENSITY = 'Medium' where STATE_CD = 'WA'
update T_OE_REF_STATE set POP_DENSITY = 'Medium' where STATE_CD = 'WI'
update T_OE_REF_STATE set POP_DENSITY = 'Medium' where STATE_CD = 'TX'
update T_OE_REF_STATE set POP_DENSITY = 'Low' where STATE_CD = 'AL'
update T_OE_REF_STATE set POP_DENSITY = 'Low' where STATE_CD = 'MO'
update T_OE_REF_STATE set POP_DENSITY = 'Low' where STATE_CD = 'WV'
update T_OE_REF_STATE set POP_DENSITY = 'Low' where STATE_CD = 'MN'
update T_OE_REF_STATE set POP_DENSITY = 'Low' where STATE_CD = 'VT'
update T_OE_REF_STATE set POP_DENSITY = 'Low' where STATE_CD = 'MI'
update T_OE_REF_STATE set POP_DENSITY = 'Low' where STATE_CD = 'AZ'
update T_OE_REF_STATE set POP_DENSITY = 'Low' where STATE_CD = 'AR'
update T_OE_REF_STATE set POP_DENSITY = 'Low' where STATE_CD = 'OK'
update T_OE_REF_STATE set POP_DENSITY = 'Low' where STATE_CD = 'IA'
update T_OE_REF_STATE set POP_DENSITY = 'Low' where STATE_CD = 'CO'
update T_OE_REF_STATE set POP_DENSITY = 'Low' where STATE_CD = 'ME'
update T_OE_REF_STATE set POP_DENSITY = 'Low' where STATE_CD = 'OR'
update T_OE_REF_STATE set POP_DENSITY = 'Low' where STATE_CD = 'UT'
update T_OE_REF_STATE set POP_DENSITY = 'Low' where STATE_CD = 'KS'
update T_OE_REF_STATE set POP_DENSITY = 'Low' where STATE_CD = 'NV'
update T_OE_REF_STATE set POP_DENSITY = 'Low' where STATE_CD = 'NE'
update T_OE_REF_STATE set POP_DENSITY = 'Low' where STATE_CD = 'ID'
update T_OE_REF_STATE set POP_DENSITY = 'Low' where STATE_CD = 'NM'
update T_OE_REF_STATE set POP_DENSITY = 'Low' where STATE_CD = 'SD'
update T_OE_REF_STATE set POP_DENSITY = 'Low' where STATE_CD = 'ND'
update T_OE_REF_STATE set POP_DENSITY = 'Low' where STATE_CD = 'MT'
update T_OE_REF_STATE set POP_DENSITY = 'Low' where STATE_CD = 'WY'
update T_OE_REF_STATE set POP_DENSITY = 'Low' where STATE_CD = 'AK'



--****************REF_REGION  *****************************************************************************************
INSERT into [T_OE_REF_REGION] values 
(1, 'EPA Region 1'),
(2, 'EPA Region 2'),
(3, 'EPA Region 3'),
(4, 'EPA Region 4'),
(5, 'EPA Region 5'),
(6, 'EPA Region 6'),
(7, 'EPA Region 7'),
(8, 'EPA Region 8'),
(9, 'EPA Region 9'),
(10, 'EPA Region 10');


--****************REF_ENTERPRISE_PLATFORM  *****************************************************************************************
INSERT INTO [T_OE_REF_ENTERPRISE_PLATFORM] values 
('Document Management System','An enterprise-level system used to track, manage and store documents in a consistent manner across departments (also known as Enterprise Document Management or Enterprise Content Management','Hyland Onbase, MS SharePoint',1,1,0,GetDate()),
('Business Analytics/Report Generation','Software designed to analyze governmental datasets to better understand the data and present using reports, graphs and charts (also known as Business Intelligence - BI)','SAS, Tableau, Crystal Reports Server',2,1,0,GetDate()),
('eReporting','An enterprise-level system that allows the agency to collect a variety of different data from the public and regulated communities. Reporting activities could include Discharge Monitoring Reports (DMR), drinking water sampling data, monthly operating reports,etc.','',3,1,0,GetDate()),
('ePermitting','An enterprise-level system that allows the agency to issue permits and authorizations accross multiple programs from a common plateform, often including forms development and regulated entity consolidation.','',4,1,0,GetDate()),
('Enterprise Portal','An enterprise level web portal that brings information together from various departments and displays to the public in a uniform way.','',5,1,0,GetDate()),
('Exchange Network Node','Software that uses standard web services to securely initiate and respond to Exchange Network requests for information. A Node is most commonly used for initiating submission of data to EPA and for providing a web service for data requests.','OpenNode, EN-Node, CGI Node',6,1,0,GetDate()),
('Identity Management System','A standardized tool or API used by the agency across multiple applications for the authentication and authorization of application users.','Thinktecture Identity Server',7,1,0,GetDate()),
('Online Payment System','A standard tool or API used by the agency and used by multiple departments for the collection of payments from the public or regulated community','',8,1,0,GetDate()),
('Forms management','Forms automation software that allows the agency to use in-house staff to quickly develop data entry forms; typically used for internal applications','Oracle Forms',9,1,0,GetDate()),
('Public Comment','Enterprise software used to collect public comments.','',10,1,0,GetDate()),
('FOIA Response Management','Enterprise software used to record and respond to FOIA requests','SmartComment',11,1,0,GetDate()),
('Integration of Geospatial capabilities into business applications','A standard tool or API used by the agency across multiple applications for the integration of GIS capabilities (i.e., displaying map layers, selecting sites, etc.) with existing applications.','ArcGIS for Javascript, Google Maps API, OpenLaters.org',12,1,0,GetDate()),
('Mobile Enterprise Application Platform','A suite of products and services that enable development of mobile applications. This term is still immature, we are interested if you are considering/using any software/service across mobile applications that is unique to development and operation mobile applications.','',13,1,0,GetDate()),
('API Hosting/Management Platform','API management platforms can act as a proxy between external users and internal systems and typically include analytics and usage reporting, API key and authorization management and live updated documentation integration.','Umbrella',14,1,0,GetDate()),
('Public Complaints (Spill, Dumping, Odors or other)','Common platform for receiving, recording, routing, workflow and/or reporting and public access for public complaints of various types','http://nwcleanairwa.gov/forms/complaint-form/',15,1,0,GetDate()),
('eSignature','Enterprise system for managing digital signatures for ePermitting, eReporting or other transactions. Here is part of the definition from CROMERR "any information in digital form that is included in or logically associated with an electronic document for the purpose of expressing the same meaning and intention as would a handwritten signature if affixed to an equivalent paper document with the same reference to the same content. The electronic document bears or has on it an electronic signature where it includes or has logically associated with it such information.','',16,1,0,GetDate()),
('Facility/Regulated Entity Integration','Platform to help manage the integration/reconciliation of facilities and their various environmental interests and regulated aspects. Often includes duplication detection, reconciliation workflow and or master data management.','',17,1,0,GetDate())
;




--****************REF_TAG_CATEGORIES  *****************************************************************************************
INSERT into [T_OE_REF_TAG_CATEGORIES] ([TAG_CAT_NAME],[TAG_CAT_DESCRIPTION],[TAG_CAT_COLOR],[CREATE_USERIDX],[CREATE_DT]) values ('Expertise', 'User expertise', 'c91010', 0, GetDate());
INSERT into [T_OE_REF_TAG_CATEGORIES] ([TAG_CAT_NAME],[TAG_CAT_DESCRIPTION],[TAG_CAT_COLOR],[CREATE_USERIDX],[CREATE_DT]) values ('Database', 'What is the primary database application in use in your agency?', 'c91010', 0, GetDate());
INSERT into [T_OE_REF_TAG_CATEGORIES] ([TAG_CAT_NAME],[TAG_CAT_DESCRIPTION],[TAG_CAT_COLOR],[CREATE_USERIDX],[CREATE_DT]) values ('App Framework', 'What is the primary application development platform in use in your agency?', 'c91010', 0, GetDate());
INSERT into [T_OE_REF_TAG_CATEGORIES] ([TAG_CAT_NAME],[TAG_CAT_DESCRIPTION],[TAG_CAT_COLOR],[CREATE_USERIDX],[CREATE_DT]) values ('Cloud', 'Are you using/considering cloud applications?', 'c91010', 0, GetDate());
INSERT into [T_OE_REF_TAG_CATEGORIES] ([TAG_CAT_NAME],[TAG_CAT_DESCRIPTION],[TAG_CAT_COLOR],[CREATE_USERIDX],[CREATE_DT]) values ('API', 'Do you have Internal/External APIs and/or an Agency Strategy for APIs?', 'c91010', 0, GetDate());
INSERT into [T_OE_REF_TAG_CATEGORIES] ([TAG_CAT_NAME],[TAG_CAT_DESCRIPTION],[TAG_CAT_COLOR],[CREATE_USERIDX],[CREATE_DT]) values ('Project Media', 'Media for Project', 'c91010', 0, GetDate());
INSERT into [T_OE_REF_TAG_CATEGORIES] ([TAG_CAT_NAME],[TAG_CAT_DESCRIPTION],[TAG_CAT_COLOR],[CREATE_USERIDX],[CREATE_DT]) values ('Project Status', 'Current Status for Project', 'c91010', 0, GetDate());
INSERT into [T_OE_REF_TAG_CATEGORIES] ([TAG_CAT_NAME],[TAG_CAT_DESCRIPTION],[TAG_CAT_COLOR],[CREATE_USERIDX],[CREATE_DT]) values ('Project Feature', 'Feature of a project', 'c91010', 0, GetDate());
INSERT into [T_OE_REF_TAG_CATEGORIES] ([TAG_CAT_NAME],[TAG_CAT_DESCRIPTION],[TAG_CAT_COLOR],[CREATE_USERIDX],[CREATE_DT]) values ('Program Area', 'Feature of a project', 'c91010', 0, GetDate());
INSERT into [T_OE_REF_TAG_CATEGORIES] ([TAG_CAT_NAME],[TAG_CAT_DESCRIPTION],[TAG_CAT_COLOR],[CREATE_USERIDX],[CREATE_DT]) values ('Use Amount', 'Amount a feature is used on a project', 'c91010', 0, GetDate());
INSERT into [T_OE_REF_TAG_CATEGORIES] ([TAG_CAT_NAME],[TAG_CAT_DESCRIPTION],[TAG_CAT_COLOR],[CREATE_USERIDX],[CREATE_DT]) values ('COTS', 'Indicate if a project is developed inhouse or not', 'c91010', 0, GetDate());


--****************REF_SYNONYMS  *****************************************************************************************
INSERT into [T_OE_REF_SYNONYMS] (SYNONYM_TEXT, [CREATE_USERIDX], [CREATE_DT]) values ('water quality,wqx,storet',0,GetDate());
INSERT into [T_OE_REF_SYNONYMS] (SYNONYM_TEXT, [CREATE_USERIDX], [CREATE_DT]) values ('attains,303d',0,GetDate());
INSERT into [T_OE_REF_SYNONYMS] (SYNONYM_TEXT, [CREATE_USERIDX], [CREATE_DT]) values ('electronic reporting rule,cromerr',0,GetDate());
INSERT into [T_OE_REF_SYNONYMS] (SYNONYM_TEXT, [CREATE_USERIDX], [CREATE_DT]) values ('frs,facility',0,GetDate());
INSERT into [T_OE_REF_SYNONYMS] (SYNONYM_TEXT, [CREATE_USERIDX], [CREATE_DT]) values ('ust,underground storage tank',0,GetDate());


--****************REF_TAGS  *****************************************************************************************
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('IT - Database Development','Expertise',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('IT - API Strategy','Expertise',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('IT - Big Data','Expertise',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('IT - Cloud-based Systems','Expertise',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('IT - Collaborative Tools','Expertise',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('IT - CROMERR','Expertise',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('IT - Distributed Software','Expertise',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('IT - Document Management','Expertise',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('IT - Exchange Network Node','Expertise',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('IT - Federated Identity Management','Expertise',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('IT - GIS and Mapping','Expertise',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('IT - LEAN','Expertise',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('IT - Mobile Development','Expertise',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('IT - Web Development','Expertise',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('IT - Agile Methodology','Expertise',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('IT - REST Services','Expertise',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('IT - Software Design','Expertise',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Water','Expertise',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Water - Drinking Water Program','Expertise',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Water - Water Quality Monitoring','Expertise',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Water - NPDES','Expertise',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Water - Compliance and Enforcement','Expertise',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Air','Expertise',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Air - Monitoring','Expertise',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Air - Permitting','Expertise',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Air - Compliance and Enforcement','Expertise',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Land','Expertise',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Land - RCRA Program','Expertise',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Land - Solid Waste','Expertise',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Model based design','Expertise',0,GetDate());

INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('SQL Server','Database',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Oracle','Database',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('MySQL','Database',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('MS Access','Database',0,GetDate());

INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('.NET','App Framework',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Java','App Framework',0,GetDate());

INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Yes - using now','Cloud',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Planned','Cloud',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Inerested','Cloud',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Not Using','Cloud',0,GetDate());

INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('I don''t know if we use APIs in our agency','API',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('We have some internal APIs','API',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('We have internal and external APIs but no ''strategy''','API',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('We have or are working on a written strategy','API',0,GetDate());

INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Air','Project Media',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Water','Project Media',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Waste','Project Media',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Health & Safety','Project Media',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Cross Program','Project Media',0,GetDate());

INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Not under consideration','Project Status',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Interested but no firm plans','Project Status',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Planning/on our radar','Project Status',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Funded but not started','Project Status',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('In development','Project Status',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('In production','Project Status',0,GetDate());

INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Billing','Project Feature',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Cloud','Project Feature',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Compliance','Project Feature',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('CROMERR','Project Feature',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Data Collection','Project Feature',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Enforcement','Project Feature',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Free','Project Feature',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Individual Certification','Project Feature',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Inspections','Project Feature',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Open Source','Project Feature',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Permitting','Project Feature',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Public Access','Project Feature',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Reporting','Project Feature',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Rules Engine','Project Feature',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('User Friendly','Project Feature',0,GetDate());
	INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Governance','Project Feature',0,GetDate());

INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Air Emissions Inventory','Program Area',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Beaches','Program Area',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Drinking Water','Program Area',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('EPCRA','Program Area',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Facility Inventory','Program Area',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Indoor Air','Program Area',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Hazardous Waste','Program Area',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('NESHAPS','Program Area',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('NPDES','Program Area',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('RCRA','Program Area',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Solid Waste','Program Area',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Title V','Program Area',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('TRI','Program Area',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('UIC','Program Area',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Waste Management','Program Area',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Water Quality','Program Area',0,GetDate());

INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('No','Use Amount',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Yes: Minor Component','Use Amount',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Yes: Major Component','Use Amount',0,GetDate());

INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('COTS','COTS',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Modified COTS','COTS',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('Custom Built by Vendor','COTS',0,GetDate());
INSERT into [T_OE_REF_TAGS] ([TAG_NAME], [TAG_CAT_NAME], [CREATE_USERIDX], [CREATE_DT]) values ('In-House','COTS',0,GetDate());



--*************************************  AGENCIES ************************************************************
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Alabama Department of Environmental Management','ADEM','AL',4,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Alabama Department of Conservation and Natural Resources','ADCNR','AL',4,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Alabama Department of Public Health','ADPH','AL',4,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Alaska Department of Environmental Conservation','ADEC','AK',10,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Alaska Department of Health and Social Services ','ADHSS','AK',10,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Arizona Department of Environmental Quality','AZDEQ','AZ',9,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Arizona Department of Health Services','AZDHS','AZ',9,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Arkansas Department of Environmental Quality','ARDEQ','AR',6,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Arkansas Department of Health','ARDOH','AR',6,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('American Samoa Environmental Protection Agency','ASEPA','AS',9,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('California Environmental Protection Agency','CALEPA','CA',9,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('California Department of Conservation','CALDEC','CA',9,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('California Department of Health Services','CALDHS','CA',9,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Colorado Department of Public Health and Environment ','CDPHE','CO',8,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Connecticut Department of Energy and Environmental Protection','CTDEEP','CT',1,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Connecticut Department of Puclic Health','CTDPH','CT',1,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Delaware Dept. of Natural Resources and Environmental Control','DNREC','DE',3,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Delaware Department of Health and Social Services','DHSS','DE',3,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('DC Department of Energy and Environment','DOEE','DC',3,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('District of Columbia Department of Health','DCDOH','DC',3,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Florida Department of Environmental Protection','FLDEP','FL',4,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Florida Department of Health','FLDOH','FL',4,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Georgia Department of Natural Resources','GDNR','GA',4,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Georgia Division of Public Health','GDPH','GA',4,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Guam Environmental Protection Agency','GEPA','GU',9,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Hawaii Department of Land and Natural Resources','DLNR','HI',9,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Hawaii Department of Health','HDOH','HI',9,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Idaho Department of Environmental Quality','IDEQ','ID',10,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Idaho Department of Water Resources','IDWR','ID',10,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Idaho Department of Health and Welfare','IDHW','ID',10,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Illinois Environmental Protection Agency','IEPA','IL',5,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Illinois Department of Natural Resources','IDNR','IL',5,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Illinois Department of Public Health','IDPH','IL',5,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Indiana Department of Environmental Management','IDEM','IN',5,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Indiana Department of Natural Resources','INDNR','IN',5,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Indiana State Department of Health','ISDH','IN',5,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Iowa Department of Natural Resources','Iowa DNR','IA',7,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Kansas Department of Health and Environment','KDHE','KS',7,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Kentucky Department for Natural Resources','KDNR','KY',4,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Kentucky Department for Environmental Protection','KDEP','KY',4,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Kentucky Department for Public Health','KDPH','KY',4,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Louisiana Department of Environmental Quality','LDEQ','LA',6,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Louisiana Department Health and Hospitals, Office of Public Health','LDHH','LA',6,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Maine Department of Environmental Protection','Maine DEP','ME',1,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Maine Department of Health and Human Services','Maine DHHS','ME',1,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Maryland Department of the Environment','MDE','MD',3,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Maryland Department of Natural Resources','Maryland DNR','MD',3,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Maryland Department of Health and Mental Hygiene','MDH','MD',3,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Massachusetts Department of Environmental Protection','MassDEP','MA',1,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Massachusetts Department of Public Health','MassDPH','MA',1,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Michigan Department of Environmental Quality','Michigan DEQ','MI',5,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Michigan Department of Public Health','MDHHS','MI',5,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Minnesota Department of Natural Resources','MDNR','MN',5,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Minnesota Pollution Control Agency','MPCA','MN',5,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Minnesota Department of Health','MDH','MN',5,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Mississippi Department of Environmental Quality','Mississippi DEQ','MS',4,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Mississippi Department of Health','MSDOH','MS',4,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Missouri Department of Natural Resources','Missouri DNR','MO',7,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Missouri Department of Conservation','Missouri DC','MO',7,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Missouri Department of Health','Missouri DOH','MO',7,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Montana Department of Environmental Quality','MTDEQ','MT',8,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Montana Department of Health','MTDOH','MT',8,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Nebraska Department of Environmental Quality','Nebraska DEQ','NE',7,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Nebraska Department of Health & Human Services','Nebraska DHHS','NE',7,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Nevada Department of Conservation and Natural Resources','Nevada DCNR','NV',9,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Nevada Division of Environmental Protection','Nevada DEP','NV',9,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('New Hampshire Department of Environmental Services','NHDES','NH',1,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('New Hampshire Department of Health and Human Services','NH DHHS','NH',1,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('New Hampshire Department of Information Technology','New Hampshire DOIT','NH',1,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('New Jersey Department of Environmental Protection','NJDEP','NJ',2,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('New Jersey Department of Health and Senior Services','NJDHHS','NJ',2,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('New Mexico Environment Department','NM ED','NM',6,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('New Mexico Department of Health','NM DOH','NM',6,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('New York State Department of Environmental Conservation','NYSDEC','NY',2,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('New York State Department of Health','NYSDOH','NY',2,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('North Carolina Department of Environmental Quality','NCDEQ','NC',4,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('North Dakota Department of Health','ND DOH','ND',8,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Ohio Environmental Protection Agency','OEPA','OH',5,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Ohio Department of Health','ODH','OH',5,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Oklahoma Department of Environmental Quality','OKDEQ','OK',6,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Oklahoma State Department of Health','OSDH','OK',6,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Oregon Department of Environmental Quality ','OR DEQ','OR',10,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Oregon Department of Fish and Wildlife','ODFW','OR',10,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Oregon Department of Human Resources','OR DHS','OR',10,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Pennsylvania Department of Environmental Protection','Pennsylvania DEP','PA',3,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Pennsylvania Department of Conservation and Natural Resources','PA DCNR','PA',3,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Pennsylvania Department of Health','PA DOH','PA',3,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Rhode Island Department of Environmental Management','RIDEM','RI',1,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Rhode Island Department of Health','RIDOH','RI',1,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('South Carolina Department of Health and Environmental Control','SC DHEC','SC',4,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('South Carolina Department of Natural Resources','SC DNR','SC',4,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('South Dakota Department of Environment and Natural Resources','SD DENR','SD',8,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('South Dakota Department of Health','SD DOH','SD',8,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Tennessee Department of Environment and Conservation','TN DEC','TN',4,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Tennessee Department of Health','TN DOH','TN',4,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Texas Commission on Environmental Quality','TCEQ','TX',6,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Texas Department of Health','Texas DOH','TX',6,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Utah Department of Environmental Quality','Utah DEQ','UT',8,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Utah Department of Health','UDOH','UT',8,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Vermont Agency of Natural Resources','VT ANR','VT',1,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Vermont Department of Environmental Conservation','VTDEC','VT',1,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Vermont Department of Health','VT DOH','VT',1,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Virginia Department of Environmental Quality','Virginia DEQ','VA',3,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Virginia Department of Health','VDH','VA',3,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Washington State Department of Ecology','WA ECY','WA',10,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Washington State Department of Natural Resources','WA DNR','WA',10,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Washington State Department of Health','WA DOH','WA',10,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('West Virginia Department of Environmental Protection','WV DEP','WV',3,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('West Virginia Department of Health and Human Resources','WV DHHR','WV',3,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Wisconsin Department of Natural Resources','Wisconsin DNR','WI',5,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Wyoming Department of Environmental Quality','Wyoming DEQ','WY',8,1)
insert into T_OE_ORGANIZATION ([ORG_NAME],[ORG_ABBR], [STATE_CD], [EPA_REGION], [ACT_IND]) values('Wyoming Department of Health','Wyoming DOH','WY',8,1)


INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'adem.alabama.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Alabama Department of Environmental Management';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'dcnr.alabama.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Alabama Department of Conservation and Natural Resources';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, '' from T_OE_ORGANIZATION where ORG_NAME = 'Alabama Department of Public Health';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'alaska.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Alaska Department of Environmental Conservation';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'alaska.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Alaska Department of Health and Social Services ';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'azdeq.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Arizona Department of Environmental Quality';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'azdhs.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Arizona Department of Health Services';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'adeq.state.ar.us' from T_OE_ORGANIZATION where ORG_NAME = 'Arkansas Department of Environmental Quality';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, '' from T_OE_ORGANIZATION where ORG_NAME = 'Arkansas Department of Health';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'epa.as.gov' from T_OE_ORGANIZATION where ORG_NAME = 'American Samoa Environmental Protection Agency';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'calepa.ca.gov' from T_OE_ORGANIZATION where ORG_NAME = 'California Environmental Protection Agency';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, '' from T_OE_ORGANIZATION where ORG_NAME = 'California Department of Conservation';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, '' from T_OE_ORGANIZATION where ORG_NAME = 'California Department of Health Services';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'state.co.us' from T_OE_ORGANIZATION where ORG_NAME = 'Colorado Department of Public Health and Environment ';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'ct.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Connecticut Department of Energy and Environmental Protection';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'ct.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Connecticut Department of Puclic Health';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'state.de.us' from T_OE_ORGANIZATION where ORG_NAME = 'Delaware Dept. of Natural Resources and Environmental Control';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'state.de.us' from T_OE_ORGANIZATION where ORG_NAME = 'Delaware Department of Health and Social Services';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'dc.gov' from T_OE_ORGANIZATION where ORG_NAME = 'DC Department of Energy and Environment';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'dc.gov' from T_OE_ORGANIZATION where ORG_NAME = 'District of Columbia Department of Health';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'dep.state.fl.us' from T_OE_ORGANIZATION where ORG_NAME = 'Florida Department of Environmental Protection';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, '' from T_OE_ORGANIZATION where ORG_NAME = 'Florida Department of Health';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'dnr.ga.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Georgia Department of Natural Resources';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, '' from T_OE_ORGANIZATION where ORG_NAME = 'Georgia Division of Public Health';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'epa.guam.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Guam Environmental Protection Agency';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, '' from T_OE_ORGANIZATION where ORG_NAME = 'Hawaii Department of Land and Natural Resources';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'doh.hawaii.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Hawaii Department of Health';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'deq.idaho.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Idaho Department of Environmental Quality';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'idwr.idaho.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Idaho Department of Water Resources';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, '' from T_OE_ORGANIZATION where ORG_NAME = 'Idaho Department of Health and Welfare';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'illinois.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Illinois Environmental Protection Agency';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'illinois.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Illinois Department of Natural Resources';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, '' from T_OE_ORGANIZATION where ORG_NAME = 'Illinois Department of Public Health';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'idem.in.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Indiana Department of Environmental Management';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, '' from T_OE_ORGANIZATION where ORG_NAME = 'Indiana Department of Natural Resources';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, '' from T_OE_ORGANIZATION where ORG_NAME = 'Indiana State Department of Health';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'dnr.iowa.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Iowa Department of Natural Resources';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'ks.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Kansas Department of Health and Environment';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'ky.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Kentucky Department for Natural Resources';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'ky.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Kentucky Department for Environmental Protection';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'ky.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Kentucky Department for Public Health';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'la.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Louisiana Department of Environmental Quality';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'la.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Louisiana Department Health and Hospitals, Office of Public Health';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'maine.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Maine Department of Environmental Protection';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'maine.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Maine Department of Health and Human Services';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'maryland.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Maryland Department of the Environment';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'maryland.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Maryland Department of Natural Resources';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'maryland.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Maryland Department of Health and Mental Hygiene';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'state.ma.us' from T_OE_ORGANIZATION where ORG_NAME = 'Massachusetts Department of Environmental Protection';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'state.ma.us' from T_OE_ORGANIZATION where ORG_NAME = 'Massachusetts Department of Public Health';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'michigan.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Michigan Department of Environmental Quality';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'michigan.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Michigan Department of Public Health';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'state.mn.us' from T_OE_ORGANIZATION where ORG_NAME = 'Minnesota Department of Natural Resources';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'state.mn.us' from T_OE_ORGANIZATION where ORG_NAME = 'Minnesota Pollution Control Agency';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'state.mn.us' from T_OE_ORGANIZATION where ORG_NAME = 'Minnesota Department of Health';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'deq.state.ms.us' from T_OE_ORGANIZATION where ORG_NAME = 'Mississippi Department of Environmental Quality';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, '' from T_OE_ORGANIZATION where ORG_NAME = 'Mississippi Department of Health';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'dnr.mo.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Missouri Department of Natural Resources';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, '' from T_OE_ORGANIZATION where ORG_NAME = 'Missouri Department of Conservation';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, '' from T_OE_ORGANIZATION where ORG_NAME = 'Missouri Department of Health';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'mt.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Montana Department of Environmental Quality';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'mt.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Montana Department of Health';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'nebraska.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Nebraska Department of Environmental Quality';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'nebraska.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Nebraska Department of Health & Human Services';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, '' from T_OE_ORGANIZATION where ORG_NAME = 'Nevada Department of Conservation and Natural Resources';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'ndep.nv.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Nevada Division of Environmental Protection';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'des.nh.gov' from T_OE_ORGANIZATION where ORG_NAME = 'New Hampshire Department of Environmental Services';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, '' from T_OE_ORGANIZATION where ORG_NAME = 'New Hampshire Department of Health and Human Services';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'doit.nh.gov' from T_OE_ORGANIZATION where ORG_NAME = 'New Hampshire Department of Information Technology';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'dep.nj.gov' from T_OE_ORGANIZATION where ORG_NAME = 'New Jersey Department of Environmental Protection';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, '' from T_OE_ORGANIZATION where ORG_NAME = 'New Jersey Department of Health and Senior Services';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'state.nm.us' from T_OE_ORGANIZATION where ORG_NAME = 'New Mexico Environment Department';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'state.nm.us' from T_OE_ORGANIZATION where ORG_NAME = 'New Mexico Department of Health';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'dec.ny.gov' from T_OE_ORGANIZATION where ORG_NAME = 'New York State Department of Environmental Conservation';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, '' from T_OE_ORGANIZATION where ORG_NAME = 'New York State Department of Health';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'ncdenr.gov' from T_OE_ORGANIZATION where ORG_NAME = 'North Carolina Department of Environmental Quality';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'nd.gov' from T_OE_ORGANIZATION where ORG_NAME = 'North Dakota Department of Health';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'epa.ohio.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Ohio Environmental Protection Agency';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, '' from T_OE_ORGANIZATION where ORG_NAME = 'Ohio Department of Health';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'deq.ok.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Oklahoma Department of Environmental Quality';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, '' from T_OE_ORGANIZATION where ORG_NAME = 'Oklahoma State Department of Health';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'deq.state.or.us' from T_OE_ORGANIZATION where ORG_NAME = 'Oregon Department of Environmental Quality ';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, '' from T_OE_ORGANIZATION where ORG_NAME = 'Oregon Department of Fish and Wildlife';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, '' from T_OE_ORGANIZATION where ORG_NAME = 'Oregon Department of Human Resources';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'pa.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Pennsylvania Department of Environmental Protection';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'pa.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Pennsylvania Department of Conservation and Natural Resources';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, '' from T_OE_ORGANIZATION where ORG_NAME = 'Pennsylvania Department of Health';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'dem.ri.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Rhode Island Department of Environmental Management';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, '' from T_OE_ORGANIZATION where ORG_NAME = 'Rhode Island Department of Health';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'dhec.sc.gov' from T_OE_ORGANIZATION where ORG_NAME = 'South Carolina Department of Health and Environmental Control';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, '' from T_OE_ORGANIZATION where ORG_NAME = 'South Carolina Department of Natural Resources';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'state.sd.us' from T_OE_ORGANIZATION where ORG_NAME = 'South Dakota Department of Environment and Natural Resources';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, '' from T_OE_ORGANIZATION where ORG_NAME = 'South Dakota Department of Health';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'tn.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Tennessee Department of Environment and Conservation';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'tn.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Tennessee Department of Health';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'tceq.texas.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Texas Commission on Environmental Quality';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, '' from T_OE_ORGANIZATION where ORG_NAME = 'Texas Department of Health';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'utah.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Utah Department of Environmental Quality';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'utah.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Utah Department of Health';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'vermont.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Vermont Agency of Natural Resources';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'vermont.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Vermont Department of Environmental Conservation';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'vermont.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Vermont Department of Health';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'deq.virginia.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Virginia Department of Environmental Quality';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'vdh.virginia.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Virginia Department of Health';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'ecy.wa.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Washington State Department of Ecology';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, '' from T_OE_ORGANIZATION where ORG_NAME = 'Washington State Department of Natural Resources';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, '' from T_OE_ORGANIZATION where ORG_NAME = 'Washington State Department of Health';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'wv.gov' from T_OE_ORGANIZATION where ORG_NAME = 'West Virginia Department of Environmental Protection';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'wv.gov' from T_OE_ORGANIZATION where ORG_NAME = 'West Virginia Department of Health and Human Resources';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'wisconsin.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Wisconsin Department of Natural Resources';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'wyo.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Wyoming Department of Environmental Quality';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'wyo.gov' from T_OE_ORGANIZATION where ORG_NAME = 'Wyoming Department of Health';


INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'massmail.state.ma.us' from T_OE_ORGANIZATION where ORG_NAME = 'Massachusetts Department of Environmental Protection';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'massmail.state.ma.us' from T_OE_ORGANIZATION where ORG_NAME = 'Massachusetts Department of Public Health';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'dep.state.nj.us' from T_OE_ORGANIZATION where ORG_NAME = 'New Jersey Department of Environmental Protection';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'state.vt.us' from T_OE_ORGANIZATION where ORG_NAME = 'Vermont Agency of Natural Resources';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'state.vt.us' from T_OE_ORGANIZATION where ORG_NAME = 'Vermont Department of Environmental Conservation';
INSERT INTO T_OE_ORGANIZATION_EMAIL_RULE (ORG_IDX,  EMAIL_STRING) select ORG_IDX, 'state.vt.us' from T_OE_ORGANIZATION where ORG_NAME = 'Vermont Department of Health';
