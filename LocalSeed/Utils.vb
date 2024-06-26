﻿Imports System.Text.RegularExpressions

Public Module Utils
    ' Type reference for finding an existing row
    Public Structure DuplicateReturnType
        Public found As Boolean
        Public loopCount As Integer
        Public isError As Boolean
        Public errorCode As Integer
        Public errorContent As String
        Public data
    End Structure

    ' function name: FindDuplicate
    ' desc: To be able to identify a duplicate field; this has been modulised to be able to use this in any part of the solution to avoid redundancy of code and increase efficiency.
    ' args: (tableName, columnIndex, tableValue) Returns Boolean
    Public Function FindByValue(ByVal DS As DS, ByVal tableName As String, ByVal columnIndex As Integer, ByVal tableValue As String) As DuplicateReturnType
        Dim dataFound As Boolean
        Dim data As DataRow
        Dim rowCount, loopCount As Integer

        ' query the database to identify number of rows
        rowCount = SignUp.DS.Tables(tableName).Rows.Count() - 1
        dataFound = False
        loopCount = 0

        ' loop through the user data fields to detect any duplicates specified in the (tableValue) parameter.
        Do While (loopCount <= rowCount) And (dataFound = False)
            data = SignUp.DS.Tables(tableName)(loopCount) ' scrape the data found at the certain (x, y) position according to the (columnIndex, tableValue) parameters
            Dim dbQuery = data(columnIndex)

            If (dbQuery = tableValue) Then ' if found exit the loop
                dataFound = True
            End If

            loopCount += 1 ' increment loop count
        Loop

#Disable Warning BC42104 ' Variable is used before it has been assigned a value. This is neccesary to take all the neccessary values
        Dim returnVal As New DuplicateReturnType With {
            .found = dataFound,
            .loopCount = loopCount - 1,
            .data = data
        }

        Return returnVal
    End Function

    ' function name: GetNextUserID
    ' desc: This will search for the next available userID to create a unique primary key hash for the user, ex: ap0001
    ' args: (DS, tableName) Returns String
    Public Function GetNextUserID(ByVal DS As DS, ByVal tableName As String) As String
        ' query the database to get the count of existing users
        Dim rows As Integer = SignUp.DS.Tables(tableName).Rows.Count
        Dim latestID As Integer = 0

        ' check if the number of rows isn't zero so we can create a new unique id, otherwise this defaults to 0000
        If rows > 0 Then
            Dim stringId As String = SignUp.DS.Tables(tableName).Rows(rows - 1)(0)
            latestID = Integer.Parse(stringId.Substring(2, 4)) 'ex: strip ap0001 -> 0001 and latestID becomes 1
        End If

        ' increment the latest ID
        Dim incrementedID As Integer = latestID + 1

        ' format the ID
        Dim formattedID As String = incrementedID.ToString("D4")
        Return formattedID
    End Function

    ' function name: GetCurrentInvestorID
    ' desc: This will search for the latest InvestorID created as it is an automatically generated value
    ' args: (DS) Returns Integer
    Public Function GetLatestID(ByVal DS As DS, ByVal tableName As String)
        ' query the database to the number of rows in investor table
        Dim rows As Integer = SignUp.DS.Tables(tableName).Rows.Count
        Dim latestID As Integer = 0

        If rows > 0 Then
            latestID = SignUp.DS.Tables(tableName).Rows(rows - 1)(0)
        End If

        Return latestID
    End Function

    ' function name: IsValidEmail
    ' desc: Validates the data entry of an email.
    ' args (email) Returns Boolean
    Function IsValidEmail(ByVal email As String) As Boolean
        ' Pattern for validating email addresses.
        Dim pattern As String = "^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$"

        ' Create a regex object to verify the email.
        Dim regex As New Regex(pattern)

        ' Check if the email matches the pattern.
        Return regex.IsMatch(email)
    End Function
End Module
