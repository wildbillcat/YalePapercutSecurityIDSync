SELECT        upi
FROM            yuapps.yuhr_current_active_people
WHERE        (net_id = 'PEM4')

--will return 1 row containing the UPI number associated with the NetID. NetIDs are stored in Upper Case Format.