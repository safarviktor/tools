param(	[string]$ConnString='', 
		[string]$Server='', 
		[string]$Database='', 
		[string]$UserName='',
		[string]$Password=''		) #Must be the first statement in your script

		
function Test-SqlConnection {
    param(
        [Parameter(Mandatory)]
        [string]$ServerName,

        [Parameter(Mandatory)]
        [string]$DatabaseName,

        [Parameter(Mandatory)]
        [string]$UserName,
		
		[Parameter(Mandatory)]
        [string]$Password
    )

    $ErrorActionPreference = 'Stop'

    try {
        $connectionString = 'Data Source={0};database={1};User ID={2};Password={3}' -f $ServerName,$DatabaseName,$UserName,$Password
        $sqlConnection = New-Object System.Data.SqlClient.SqlConnection $ConnectionString
        $sqlConnection.Open()
        ## This will run if the Open() method does not throw an exception
        Write-Host 'Success' -ForegroundColor Green
    } catch {
        Write-Host 'NOT Success' -ForegroundColor Red
		Write-Host $_.Exception.Message
    } finally {
        ## Close the connection when we're done
        $sqlConnection.Close()
    }
}
		
function Test-SqlConnection-ConnString {
    param(
        [Parameter(Mandatory)]
        [string]$ConnectionString
    )

    $ErrorActionPreference = 'Stop'

    try {                
        $sqlConnection = New-Object System.Data.SqlClient.SqlConnection $ConnectionString
        $sqlConnection.Open()
        ## This will run if the Open() method does not throw an exception
        Write-Host 'Success' -ForegroundColor Green
    } catch {
        Write-Host 'NOT Success' -ForegroundColor Red
		Write-Host $_.Exception.Message
    } finally {        
        $sqlConnection.Close()
    }
}


If ((-Not $PSBoundParameters.ContainsKey('ConnString')) -and (-Not $PSBoundParameters.ContainsKey('Server')) )
{
	Write-Host 'Required parameters: '
	Write-Host 'Option A: ConnString'
	Write-Host 'Option B: Server, Database, UserName, Password'
}
Else
{
	If ($PSBoundParameters.ContainsKey('ConnString'))
	{
		Write-Host 'Testing SQL connection for connection string'
		Write-Host $ConnString
		Test-SqlConnection-ConnString -ConnectionString $ConnString
	}
	Else
	{
		Write-Host 'Testing SQL connection for'
		Write-Host 'Server: ' $Server
		Write-Host 'Database: ' $Database
		Write-Host 'UserName: ' $UserName
		Test-SqlConnection -ServerName $Server -DatabaseName $Database -UserName $UserName -Password $Password
	}
}































