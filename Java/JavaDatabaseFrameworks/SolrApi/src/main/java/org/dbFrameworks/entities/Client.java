package org.dbFrameworks.entities;

import org.apache.solr.client.solrj.beans.Field;

import java.util.Date;
import java.util.List;

public class Client {
    @Field("id") public String id;
    @Field("name") public String name;
    @Field("last_name") public String lastName;
    @Field("salary") public double Salary;
    @Field("description") public String description;
    @Field("age") public int age ;
    @Field("favourite_product") public int favouriteProduct;
    @Field("recent_bought_products") public List<String> recentBoughtProducts;
    @Field("birth_date") public Date birthDate;
//    @Field("has_children_b")  public boolean hasChildren = false;
//    @Field("how_many_cats_i")  public boolean howManyCats = false;
//    @Field("has_partner_b")  public boolean hasPartner = false;
}
