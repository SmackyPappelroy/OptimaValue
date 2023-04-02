# OptimaValue

## Innehåll
* [Skapa service](#"skapa-service")
* [Visa logvärde i samma graf](#visa-logvärde-i-samma-graf)

## Skapa service
Run in powershell:
 ```
$params = @{
  Name = "Optima"
  BinaryPathName = '"C:\Program Files\Optima\Optima\OptimaValue.Service.exe"'
  DisplayName = "OptimaValue"
  StartupType = "Manual"
  Description = "Optima databatchloggning för Anläggningsdata"
}
New-Service @params
 ```

## Visa logvärde i samma graf: 
 ```
    SELECT 
    lv.tag_id,
    CASE 
        WHEN lv.tag_id = 35 THEN 'Allowed' 
        WHEN lv.tag_id = 36 THEN 'Utpumpat' 
    END AS TagName, 
    COALESCE(lv.numericValue, 
             (SELECT TOP 1 lv2.numericValue 
              FROM McValueLog.dbo.logValues lv2 
              WHERE lv2.tag_id = lv.tag_id AND lv2.logTime < lv.logTime 
              ORDER BY lv2.logTime desc), 0) AS Value,
    lv.logTime,
    (SELECT TOP 1 COALESCE(lv2.numericValue, 0) 
     FROM McValueLog.dbo.logValues lv2 
     WHERE lv2.tag_id = 35 AND lv2.logTime <= lv.logTime 
     ORDER BY lv2.logTime desc) AS Allowed,
    (SELECT TOP 1 COALESCE(lv2.numericValue, 0) 
     FROM McValueLog.dbo.logValues lv2 
     WHERE lv2.tag_id = 36 AND lv2.logTime <= lv.logTime 
     ORDER BY lv2.logTime desc) AS Utpumpat
FROM McValueLog.dbo.logValues lv
WHERE lv.tag_id IN (35, 36) AND lv.logTime BETWEEN '2022-01-01' AND '2024-01-31'
ORDER BY lv.logTime asc;  
```

## Kolla om en kurva är stigande eller fallande
```
set @slope = (select (SUM(CAST(numericValue AS decimal) * CAST(DATEDIFF(s, '19700101', logtime) AS decimal(18,0)) / 86400) - SUM(CAST(numericValue AS decimal)) * SUM(CAST(DATEDIFF(s, '19700101', logtime) AS decimal(18,0)) / 86400) / COUNT(*)) / 
         (SUM(CAST(DATEDIFF(s, '19700101', logtime) AS decimal(18,0)) / 86400 * CAST(DATEDIFF(s, '19700101', logtime) AS decimal(18,0)) / 86400) - SUM(CAST(DATEDIFF(s, '19700101', logtime) AS decimal(18,0)) / 86400) * SUM(CAST(DATEDIFF(s, '19700101', logtime) AS decimal(18,0)) / 86400) / COUNT(*))
		FROM McValueLog.dbo.logValues
		WHERE logtime between @startDate and dateadd(day,1,@startDate)  and tag_id = 34
		AND numericValue IS NOT NULL 
		AND numericValue != 0)

SELECT 
  CASE 
    WHEN @slope > 0
    THEN 'Stigande flöde'
    WHEN @slope = 0
    THEN 'Konstanst flöde'
    ELSE 'Minskande flöde' 
  END AS trend_status 
```
