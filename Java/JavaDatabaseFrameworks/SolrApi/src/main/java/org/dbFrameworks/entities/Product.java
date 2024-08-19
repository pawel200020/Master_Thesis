package org.dbFrameworks.entities;

import org.apache.solr.client.solrj.beans.Field;

public class Product {
    public Product(String id, String name, String description, double price, String category) {
        this.id = id;
        this.name = name;
        this.description = description;
        this.price = price;
        this.category = category;
    }
    public Product() {}
    @Field("id") public String id;
    @Field("name") public String name;
    @Field("description")public String description;
    @Field("price") public double price;
    @Field("category") public String category;

}
