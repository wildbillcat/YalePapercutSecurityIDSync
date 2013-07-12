SELECT        bid, first_name, last_name, upi, bid_format_id
FROM          pic_prod.badge_validation_v
WHERE        (logical_status = 'ACTIVE') AND (upi = '13410149')

--will return 1 row for each BID format. 1 = Mag Strip, 2 = Prox, 3 = SEAS 