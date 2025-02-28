﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ElectronicObserver.Resource;
using ElectronicObserver.Resource.Record;
using Translation = ElectronicObserver.Properties.Window.Dialog.DialogAlbumShipParameter;

namespace ElectronicObserver.Window.Dialog;

public partial class DialogAlbumShipParameter : Form
{

	public DialogAlbumShipParameter()
	{
		InitializeComponent();

		Translate();
	}

	public DialogAlbumShipParameter(int shipID)
		: this()
	{

		InitView(shipID);
	}

	public void Translate()
	{
		ParameterView_Key.HeaderText = EncycloRes.ParameterView_Key;
		ParameterView_Value.HeaderText = EncycloRes.ParameterView_Value;
		ButtonCancel.Text = GeneralRes.Cancel;
		label1.Text = EncycloRes.EditShipParameterHint;
		Text = EncycloRes.EditShipParameter;
	}

	private void DialogAlbumShipParameter_Load(object sender, EventArgs e)
	{
		this.Icon = ResourceManager.ImageToIcon(ResourceManager.Instance.Icons.Images[(int)IconContent.FormAlbumShip]);
	}


	private void InitView(int shipID)
	{

		var record = RecordManager.Instance.ShipParameter[shipID];

		if (record == null)
		{
			RecordManager.Instance.ShipParameter[shipID] = record = new ShipParameterRecord.ShipParameterElement();
		}


		var keys = RecordManager.Instance.ShipParameter.RecordHeader.Split(',');
		var values = record.SaveLine().Split(',');

		ParameterView.Rows.Clear();
		var rows = new DataGridViewRow[keys.Length];

		for (int i = 0; i < rows.Length; i++)
		{
			rows[i] = new DataGridViewRow();
			rows[i].CreateCells(ParameterView);
			rows[i].SetValues(keys[i], values[i]);
		}

		rows[0].ReadOnly = rows[1].ReadOnly = true;

		ParameterView.Rows.AddRange(rows);

	}



	private void ButtonOK_Click(object sender, EventArgs e)
	{

		try
		{

			var record = new ShipParameterRecord.ShipParameterElement();

			var sb = new StringBuilder();

			foreach (DataGridViewRow row in ParameterView.Rows)
			{
				sb.Append(row.Cells[ParameterView_Value.Index].Value + ",");
			}
			sb.Remove(sb.Length - 1, 1);

			record.LoadLine(sb.ToString());

			RecordManager.Instance.ShipParameter[record.ShipID] = record;


		}
		catch (Exception ex)
		{

			MessageBox.Show(Translation.FailedToSetParameter + "\r\n" + ex.Message, Translation.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}


		Close();
	}

	private void ButtonCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void DialogAlbumShipParameter_FormClosed(object sender, FormClosedEventArgs e)
	{
		ResourceManager.DestroyIcon(Icon);
	}
}
