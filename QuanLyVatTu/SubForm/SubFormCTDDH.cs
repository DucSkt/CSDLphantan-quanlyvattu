﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace QuanLyVatTu.SubForm
{
    public partial class SubFormCTDDH : DevExpress.XtraEditors.XtraForm
    {
        //Cờ đánh dấu đã thêm thành công thì khi Closing cho EndEdit không thì CancelEdit
        private bool flagSuccess = false;
        public SubFormCTDDH()
        {
            InitializeComponent();
        }
        private void SubFormCTDDH_Load(object sender, EventArgs e)
        {
            this.vattuTableAdapter.Connection.ConnectionString = Program.connectionString;
            this.cTDDHTableAdapter.Connection.ConnectionString = Program.connectionString;
            this.vattuTableAdapter.Fill(this.qLVTDataSet.Vattu);
            this.cTDDHTableAdapter.Fill(this.qLVTDataSet.CTDDH);
            this.cTDDHBDS.DataSource = Program.formNhanVien.getCTDatHangBDS();
        }
        private void SubFormCTDDH_Shown(object sender, EventArgs e)
        {
            ////Đặt trong Event Load thì Thread kết thúc sớm hơn Fill nên không đồng bộ, nên dùng Event có thứ tự sau
            this.cTDDHBDS.AddNew();

            /* 
             * Không hiểu sao không cập nhật MasoDDH trong CTDDH mà lại tự sinh ra MasoDDH,
             * nên chỗ này nếu ta set MasoDDH lần nữa thì sẽ phát sinh lỗi EndEdit không được      
            */
            BindingSource tempDDH = Program.formNhanVien.getDatHangBDS();
            string valueMasoDDH = getDataRow(tempDDH, "MasoDDH");
            tbMasoDDH.Text = valueMasoDDH;
            tbMaVT.Text = ((DataRowView)vatTuBDS[vatTuBDS.Position])["MAVT"].ToString().Trim();
            //setDataRow(this.cTDDHBDS, "MasoDDH", valueMasoDDH);
            ((DataRowView)cTDDHBDS[cTDDHBDS.Position])["SOLUONG"] = 1;
            ((DataRowView)cTDDHBDS[cTDDHBDS.Position])["DONGIA"] = 0;
            nuSoLuong.Value = 1;
            nuDonGia.Value = 0;
        }
        private void SubFormCTDDH_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (flagSuccess == false) cTDDHBDS.CancelEdit();
            Program.formMain.Enabled = true;
        }
        private bool checkValidate(TextBox tb, string str)
        {
            if (tb.Text.Trim().Equals(""))
            {
                MessageBox.Show(str, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tb.Focus();
                return false;
            }
            return true;
        }
        private void gvVatTu_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            tbMaVT.Text = getDataRow(vatTuBDS, "MAVT");
        }
        private void btnOK_Click(object sender, EventArgs e)
        {          
            if (!checkValidate(tbMaVT, "Mã VT is not empty!")) return;
            if (nuSoLuong.Value == 0)
            {
                MessageBox.Show("Số lượng phải lớn hơn 0!", "Thông báo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (nuDonGia.Value == 0)
            {
                MessageBox.Show("Đơn giá phải lớn hơn 0!", "Thông báo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //Kiểm tra trùng CTDDH
            int positionMaVT = cTDDHBDS.Find("MAVT", tbMaVT.Text);
            if (positionMaVT != -1 && (positionMaVT != cTDDHBDS.Position))
            {
                MessageBox.Show("Trùng Mã số DDH & Mã VT của chi tiết Đơn Đặt Hàng khác!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult dr = MessageBox.Show("Bạn có chắc muốn ghi dữ liệu vào Database?", "Thông báo",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                try
                {
                    this.cTDDHBDS.EndEdit();
                    this.cTDDHTableAdapter.Update(Program.formNhanVien.getDataSet().CTDDH);
                    flagSuccess = true;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ghi dữ liệu thất lại. Vui lòng kiểm tra lại!\n" + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //Lỗi cho New lại cái mới(nếu không dữ liệu lúc này là Position của phần tử cuối)
                    this.cTDDHBDS.AddNew();
                    tbMaVT.Text = getDataRow(vatTuBDS, "MAVT");
                    nuSoLuong.Value = 1;
                    nuDonGia.Value = 0;
                }
            }
        }
        
        private string getDataRow(BindingSource bindingSource, string column)
        {
            return ((DataRowView)bindingSource[bindingSource.Position])[column].ToString().Trim();
        }
    }
}