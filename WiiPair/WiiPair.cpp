#include "stdafx.h"

#include <windows.h>
#include <bthsdpdef.h>
#include <bthdef.h>
#include <BluetoothAPIs.h>
#include <strsafe.h>
#include <iostream>

#pragma comment(lib, "Bthprops.lib")

DWORD ShowErrorCode(LPTSTR msg, DWORD dw)
{
	// Retrieve the system error message for the last-error code

	LPVOID lpMsgBuf;

	FormatMessage(
		FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
		NULL,
		dw,
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
		(LPTSTR)&lpMsgBuf,
		0,
		NULL
	);

	_tprintf(_T("%s: %s"), msg, lpMsgBuf);

	LocalFree(lpMsgBuf);

	return dw;
}

_TCHAR * FormatBTAddress(BLUETOOTH_ADDRESS address)
{
	static _TCHAR ret[20];
	_stprintf(ret, _T("%02x:%02x:%02x:%02x:%02x:%02x"),
		address.rgBytes[5],
		address.rgBytes[4],
		address.rgBytes[3],
		address.rgBytes[2],
		address.rgBytes[1],
		address.rgBytes[0]
	);
	return ret;
}

int _tmain(int argc, _TCHAR* argv[])
{
	bool pairVia12 = true;
	bool deleteWiimotes = false;
	{
		std::cout << "Remove found wiimotes? y/n: ";
		char answer;
		std::cin >> answer;
		if (answer == 'y' || answer == 'Y')
		{
			deleteWiimotes = true;
		}
	}
	{
		std::cout << "Are you starting pair request via 'Sync' button? y/n: ";
		char answer;
		std::cin >> answer;
		bool deleteWiimotes = false;
		if (answer == 'y' || answer == 'Y')
		{
			pairVia12 = false;
		}
	}
	HANDLE hRadio;
	int nPaired = 0;
	///////////////////////////////////////////////////////////////////////
	// Enumerate BT radios
	///////////////////////////////////////////////////////////////////////
	{
		HBLUETOOTH_RADIO_FIND hFindRadio;
		BLUETOOTH_FIND_RADIO_PARAMS radioParam;

		_tprintf(_T("Enumerating radios...\n"));

		radioParam.dwSize = sizeof(BLUETOOTH_FIND_RADIO_PARAMS);

		hFindRadio = BluetoothFindFirstRadio(&radioParam, &hRadio);
		if (hFindRadio)
		{
			BluetoothFindRadioClose(hFindRadio);
		}
		else
		{
			ShowErrorCode(_T("Error enumerating radios"), GetLastError());
			Sleep(4000);
			return (1);
		}
		_tprintf(_T("Found a radio\n"));
	}

	///////////////////////////////////////////////////////////////////////
	// Keep looping until we pair with a Wii device
	///////////////////////////////////////////////////////////////////////


	BLUETOOTH_RADIO_INFO radioInfo;
	HBLUETOOTH_DEVICE_FIND hFind;
	BLUETOOTH_DEVICE_INFO btdi;
	BLUETOOTH_DEVICE_SEARCH_PARAMS srch;

	radioInfo.dwSize = sizeof(radioInfo);
	btdi.dwSize = sizeof(btdi);
	srch.dwSize = sizeof(BLUETOOTH_DEVICE_SEARCH_PARAMS);

	ShowErrorCode(_T("BluetoothGetRadioInfo"), BluetoothGetRadioInfo(hRadio, &radioInfo));

	_tprintf(_T("Radio %d: %ls %s(Inverted)\n"),
		"1",
		radioInfo.szName,
		FormatBTAddress(radioInfo.address)
	);

	srch.fReturnAuthenticated = TRUE;
	srch.fReturnRemembered = TRUE;
	srch.fReturnConnected = TRUE;
	srch.fReturnUnknown = TRUE;
	srch.fIssueInquiry = TRUE;
	srch.cTimeoutMultiplier = 2;
	srch.hRadio = hRadio;


	WCHAR pass[6];
	if (!pairVia12) {
		// MAC address is passphrase of host with the sync button
		pass[0] = radioInfo.address.rgBytes[0];
		pass[1] = radioInfo.address.rgBytes[1];
		pass[2] = radioInfo.address.rgBytes[2];
		pass[3] = radioInfo.address.rgBytes[3];
		pass[4] = radioInfo.address.rgBytes[4];
		pass[5] = radioInfo.address.rgBytes[5];
	}

	while (nPaired == 0)
	{

		_tprintf(_T("Scanning...\n"));

		hFind = BluetoothFindFirstDevice(&srch, &btdi);

		if (hFind == NULL)
		{
			if (GetLastError() == ERROR_NO_MORE_ITEMS)
			{
				_tprintf(_T("No bluetooth devices found.\n"));
			}
			else
			{
				ShowErrorCode(_T("Error enumerating devices"), GetLastError());
				return (1);
			}
		}
		else
		{
			do
			{
				if (!wcscmp(btdi.szName, L"Nintendo RVL-WBC-01") || !wcscmp(btdi.szName, L"Nintendo RVL-CNT-01") || !wcscmp(btdi.szName, L"Nintendo RVL-CNT-01-TR"))
				{

					_tprintf(_T("Found Nintendo device: %s\n"), btdi.szName);

					DWORD pcServices = 16;
					GUID guids[16];
					BOOL error = FALSE;

					if (!error)
					{
						if (btdi.fRemembered && deleteWiimotes)
						{
							// Make Windows forget pairing
							if (ShowErrorCode(_T("BluetoothRemoveDevice"), BluetoothRemoveDevice(&btdi.Address)) != ERROR_SUCCESS)
								error = TRUE;
						} else if (btdi.fRemembered && !deleteWiimotes) {
							_tprintf(_T("%s is already connected.\n"), btdi.szName);
							continue;
						}
					}


					if (!error)
					{
						if (pairVia12) {
							// MAC address is passphrase of Wiimote in 1+2 button mode
							pass[0] = btdi.Address.rgBytes[0];
							pass[1] = btdi.Address.rgBytes[1];
							pass[2] = btdi.Address.rgBytes[2];
							pass[3] = btdi.Address.rgBytes[3];
							pass[4] = btdi.Address.rgBytes[4];
							pass[5] = btdi.Address.rgBytes[5];
						}
						// Pair with Wii device
						if (ShowErrorCode(_T("BluetoothAuthenticateDevice"), BluetoothAuthenticateDevice(NULL, hRadio, &btdi, pass, 6)) != ERROR_SUCCESS)
							error = TRUE;
					}

					if (!error)
					{
						// If this is not done, the Wii device will not remember the pairing
						if (ShowErrorCode(_T("BluetoothEnumerateInstalledServices"), BluetoothEnumerateInstalledServices(hRadio, &btdi, &pcServices, guids)) != ERROR_SUCCESS)
							error = TRUE;
					}

					if (!error)
					{
						// Activate service
						if (ShowErrorCode(_T("BluetoothSetServiceState"), BluetoothSetServiceState(hRadio, &btdi, &HumanInterfaceDeviceServiceClass_UUID, BLUETOOTH_SERVICE_ENABLE)) != ERROR_SUCCESS)
							error = TRUE;
					}

					if (!error)
					{
						nPaired++;
					}
				}
				else { // if (!wcscmp(btdi.szName, L"Nintendo RVL-WBC-01") || !wcscmp(btdi.szName, L"Nintendo RVL-CNT-01"))
					_tprintf(_T("Found: '%s'. Ignoring\n"), btdi.szName);
				}
			} while (BluetoothFindNextDevice(hFind, &btdi));
		} // if (hFind == NULL)

		Sleep(1000);
	}

	///////////////////////////////////////////////////////////////////////
	// Clean up
	///////////////////////////////////////////////////////////////////////

	CloseHandle(hRadio);

	_tprintf(_T("=============================================\n"), nPaired);
	_tprintf(_T("%d Wii devices paired\n"), nPaired);

	return 0;
}
