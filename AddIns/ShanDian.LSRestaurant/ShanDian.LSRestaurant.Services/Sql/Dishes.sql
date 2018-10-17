--
-- 由SQLiteStudio v3.1.1 产生的文件 周一 9月 3 18:55:35 2018
--
-- 文本编码：UTF-8
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- 表：tb_categorydish
DROP TABLE IF EXISTS tb_categorydish; 
CREATE TABLE tb_categorydish (id INTEGER PRIMARY KEY ASC UNIQUE, name VARCHAR (50) UNIQUE, sort INTEGER, sortTime DATETIME, isHidden INTEGER NOT NULL DEFAULT (0), isPractice INTEGER NOT NULL DEFAULT (0), isCharging INTEGER NOT NULL DEFAULT (0), createTime DATETIME NOT NULL DEFAULT (datetime('now', 'localtime')), creator VARCHAR (50), modificationTime DATETIME NOT NULL DEFAULT (datetime('now', 'localtime')), modifitor VARCHAR (50));

-- 表：tb_childdish
DROP TABLE IF EXISTS tb_childdish; 
CREATE TABLE tb_childdish (id INTEGER PRIMARY KEY UNIQUE, dishGroupId INTEGER, dishId INTEGER, skusId INTEGER, amount DECIMAL, sort INTEGER, createTime DATETIME NOT NULL DEFAULT (datetime('now', 'localtime')), creator VARCHAR (50), modificationTime DATETIME NOT NULL DEFAULT (datetime('now', 'localtime')), modifitor VARCHAR (50));

-- 表：tb_dbhistory
DROP TABLE IF EXISTS tb_dbhistory; 
CREATE TABLE tb_dbhistory (id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, versionCode VARCHAR (30), updateTime DATETIME NOT NULL DEFAULT (datetime('now', 'localtime')));
INSERT INTO tb_dbhistory (id, versionCode, updateTime) VALUES (1, '1.0.0.0', '2018-09-03 18:54:47');

-- 表：tb_dish
DROP TABLE IF EXISTS tb_dish; 
CREATE TABLE tb_dish (id INTEGER PRIMARY KEY UNIQUE, code VARCHAR (50), name VARCHAR (100), pinYin VARCHAR (50), price INTEGER DEFAULT (0) NOT NULL, dishType INTEGER DEFAULT (10) NOT NULL, categoryId INTEGER, unit VARCHAR (20), isOnline INTEGER NOT NULL DEFAULT (1), isDiscount INTEGER NOT NULL DEFAULT (1), sort INTEGER, sortTime DATETIME, boxFee INTEGER NOT NULL DEFAULT (0), isSkus INTEGER NOT NULL DEFAULT (0), isPractice INTEGER NOT NULL DEFAULT (1), isCharging INTEGER NOT NULL DEFAULT (1), createTime DATETIME NOT NULL DEFAULT (datetime('now', 'localtime')), creator VARCHAR (50), modificationTime DATETIME NOT NULL DEFAULT (datetime('now', 'localtime')), modifitor VARCHAR (50));

-- 表：tb_dishgroup
DROP TABLE IF EXISTS tb_dishgroup; 
CREATE TABLE tb_dishgroup (id INTEGER PRIMARY KEY UNIQUE, name VARCHAR (50), dishId INTEGER, categoryId INTEGER, groupType INTEGER, sort INTEGER, dishRule VARCHAR (250), createTime DATETIME NOT NULL DEFAULT (datetime('now', 'localtime')), creator VARCHAR (50), modificationTime DATETIME NOT NULL DEFAULT (datetime('now', 'localtime')), modifitor VARCHAR (50));

-- 表：tb_estimate
DROP TABLE IF EXISTS tb_estimate; 
CREATE TABLE tb_estimate (id INTEGER PRIMARY KEY UNIQUE, dishId INTEGER, amount DECIMAL NOT NULL DEFAULT (0), createTime DATETIME NOT NULL DEFAULT (datetime('now', 'localtime')), creator VARCHAR (50), modificationTime DATETIME NOT NULL DEFAULT (datetime('now', 'localtime')), modifitor VARCHAR (50));

-- 表：tb_practicedish
DROP TABLE IF EXISTS tb_practicedish; 
CREATE TABLE tb_practicedish (id INTEGER PRIMARY KEY UNIQUE, dishId INTEGER, categoryId INTEGER, practiceGroupId INTEGER, sort INTEGER, dishRule VARCHAR (250), createTime DATETIME NOT NULL DEFAULT (datetime('now', 'localtime')), creator VARCHAR (50), modificationTime DATETIME NOT NULL DEFAULT (datetime('now', 'localtime')), modifitor VARCHAR (50));

-- 表：tb_practicegroup
DROP TABLE IF EXISTS tb_practicegroup; 
CREATE TABLE tb_practicegroup (id INTEGER PRIMARY KEY UNIQUE, name VARCHAR (50) UNIQUE, sort INTEGER NOT NULL, sortTime DATETIME, createTime DATETIME NOT NULL DEFAULT (datetime('now', 'localtime')), creator VARCHAR (50), modificationTime DATETIME NOT NULL DEFAULT (datetime('now', 'localtime')), modifitor VARCHAR (50));

-- 表：tb_practiceinfo
DROP TABLE IF EXISTS tb_practiceinfo; 
CREATE TABLE tb_practiceinfo (id INTEGER PRIMARY KEY UNIQUE, practiceGroupId INTEGER, name VARCHAR (50), price INTEGER, sort INTEGER, createTime DATETIME NOT NULL DEFAULT (datetime('now', 'localtime')), creator VARCHAR (50), modificationTime DATETIME NOT NULL DEFAULT (datetime('now', 'localtime')), modifitor VARCHAR (50));

-- 表：tb_remarks
DROP TABLE IF EXISTS tb_remarks; 
CREATE TABLE tb_remarks (id INTEGER PRIMARY KEY UNIQUE, info VARCHAR (50), createTime DATETIME NOT NULL DEFAULT (datetime('now', 'localtime')), creator VARCHAR (50), modificationTime DATETIME NOT NULL DEFAULT (datetime('now', 'localtime')), modifitor VARCHAR (50));

-- 表：tb_skusdish
DROP TABLE IF EXISTS tb_unitdef; 
CREATE TABLE tb_skusdish (skusId INTEGER PRIMARY KEY UNIQUE, dishId INTEGER, name VARCHAR (20), price INTEGER, sort INTEGER, createTime DATETIME NOT NULL DEFAULT (datetime('now', 'localtime')), creator VARCHAR (50), modificationTime DATETIME NOT NULL DEFAULT (datetime('now', 'localtime')), modifitor VARCHAR (50));

-- 表：tb_unitdef
DROP TABLE IF EXISTS tb_unitdef; 
CREATE TABLE tb_unitdef (id INTEGER PRIMARY KEY UNIQUE, name VARCHAR (50) UNIQUE, sort INTEGER, sortTime DATETIME, createTime DATETIME NOT NULL DEFAULT (datetime('now', 'localtime')), creator VARCHAR (50), modificationTime DATETIME NOT NULL DEFAULT (datetime('now', 'localtime')), modifitor VARCHAR (50));
INSERT INTO tb_unitdef (id, name, sort, sortTime, createTime, creator, modificationTime, modifitor) VALUES (1050153369370171927, '份', 1, '2018-08-08 10:01:41.7141743', '2018-08-08 10:01:41.7141743', '默认', '2018-08-08 10:01:41.7141743', '默认');

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
