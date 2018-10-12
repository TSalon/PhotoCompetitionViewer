# PhotoCompetitionViewer
Windows application to view a photo competition

Expects to find c:\Competitions that contains a zip file for each competition (currently max 2)
The zip file should be named "2018-10-12 Competition Name.zip"

Zip file should contain Competition.xml at the root level, with the following format:

    :::text
    <Competition>
      <Images>
        <Image>Tim Sawyer/221_2_Reflective.jpg</Image>
        <Image>Tim Sawyer/221_1_Bridgewater.jpg</Image>
        <Image>Tim Sawyer/221_3_Young Red Kite.jpg</Image>
      </Images>
    </Competition>

This defines the order in which the images are shown.

The zip file root level should contain a folder for each participant with images with the filename style shown above.	
	
Extracts the competitions into c:\CompetitionsExtract.
