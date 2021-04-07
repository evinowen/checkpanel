# checkpanel

An attempt at a sort of automated to-do list tracker, built in ASP.NET MVC.

SMS_TELEPHONE
	Input the telephone number provided in E.164 international standard format. (e.g.: +14255550123) that should receive notifcations from the system.

TOTP_SECRET
	Create a TOTP secret code for use when accessing the system.

API_KEY
	Choose a random, secure key to share with **checkpanel-functions**.

AZURE_QUEUE_CONNECTION
	Found under "Shared access policies".

AZURE_QUEUE_SEND_DEADLINE_NOTICE
	Should generally be **send-deadline-notice**.

AZURE_QUEUE_SEND_DAILY_REPORT
	Should generally be **send-daily-report**.

WEBSITE_TIME_ZONE
	This should be set to whatever timezone the app will need to run in.
