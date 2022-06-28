# Steps to convert Streetlights.xlsx:
#
# 1) Load file/save file as Streetlights.CSV.
#
# 2) Perform eastings/northings to lat/lon conversion. To execute script "\python27\python.exe" "\Development\Xamarin\ReportIt\ReportIt\ReportIt\StreetlightsConv.py"
#
import pandas as pd
from convertbng.util import convert_bng, convert_lonlat

streetlights = pd.read_csv("C:\Development\Xamarin\ReportIt\ReportIt\ReportIt\Streetlights.csv")

bHeadTest = False#True

listDrop = []
for index in range(0, streetlights.shape[0]):

	row = streetlights.iloc[index]

	if bHeadTest:
		print str(row['easting']) + " --- " + str(row['northing'])

	bError = False

	try:
		res = convert_lonlat(row['easting'], row['northing'])

	except:
		bError = True

	if not res:
		bError = True
		print "Conversion error at central_asset_id: " + str(row['central_asset_id'])

	if pd.isna(res[1]) or not res[1]:
		bError = True
		print "Conversion [latitude] error at central_asset_id: " + str(row['central_asset_id'])

	if pd.isna(res[0]) or not res[0]:
		bError = True
		print "Conversion [longitude] error at central_asset_id: " + str(row['central_asset_id'])

	if bError == False:
		if bHeadTest:
			print res[1]
			print res[0]

			if index == 5:
				break

		streetlights.loc[index, 'latitude'] = res[1]
		streetlights.loc[index, 'longitude'] = res[0]
	else:
		listDrop.append(index)

if bHeadTest:
	print streetlights.head()

print "Conversion complete, " + str(listDrop.count) + " records removed. Now writing output CSV"

streetlights.drop(listDrop, axis=0, inplace=True)
streetlights.to_csv("C:\Development\Xamarin\ReportIt\ReportIt\ReportIt\StreetlightsGPS.csv", index=False)
