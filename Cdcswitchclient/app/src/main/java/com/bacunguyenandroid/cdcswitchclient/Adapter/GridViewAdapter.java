package com.bacunguyenandroid.cdcswitchclient.Adapter;

import android.content.Context;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.graphics.drawable.Drawable;
import android.media.Image;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.ImageView;
import android.widget.TextView;

import com.bacunguyenandroid.cdcswitchclient.R;

import java.util.List;

public class GridViewAdapter extends BaseAdapter {


    private  List<PackageInfo> packageInfos;
    private  Context context;
    private PackageManager packageManager;

    public GridViewAdapter(Context context, List<PackageInfo> packageInfos) {
        this.packageInfos = packageInfos;
        this.context = context;
        this.packageManager = context.getPackageManager();
    }

    @Override
    public int getCount() {
       return  packageInfos.size();
    }

    @Override
    public Object getItem(int i) {
        return packageInfos.get(i);
    }

    @Override
    public long getItemId(int i) {
        return i;
    }

    @Override
    public View getView(int i, View view, ViewGroup viewGroup) {
       ViewHolder viewHolder;
        PackageInfo packageInfo = packageInfos.get(i);
        String appName = packageInfo.applicationInfo.loadLabel(packageManager).toString();
        Drawable icon =  packageInfo.applicationInfo.loadIcon(packageManager);
        if(view == null){
            view = LayoutInflater.from(context).inflate(R.layout.itemviewapp,null);
            viewHolder = new ViewHolder();
            viewHolder.AppName = (TextView)view.findViewById(R.id.appName);
            viewHolder.AppImage =  (ImageView)view.findViewById(R.id.appIcon);
            view.setTag(viewHolder);
        }else {
            viewHolder = (ViewHolder)view.getTag();
        }
        viewHolder.AppImage.setImageDrawable(icon);
        viewHolder.AppName.setText(appName);
        return view;
    }
    private  class  ViewHolder{
        private TextView AppName;
        private ImageView AppImage;
    }

}
