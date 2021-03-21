<template>
    <div  id="bd-docs-sidebar" class="bd-toc d-none d-sm-block" style="border-left: none">
        <div v-if="!isShow" id="class-definition-sidebar">
            <div class="pr-3 pl-3 pt-1 pb-1">
                <b-form-group
                    id="fieldset-is-main-class"
                    label-cols-xs="8"
                    label-cols-sm="8"
                    label-cols-lg="8"
                    label="Is Main Class"
                    label-for="input-is-main-class"
                    >
                    <b-form-checkbox id="input-is-main-class" v-model="getClassDefinition.isMainClass"></b-form-checkbox>
                </b-form-group>
                <b-form-group
                    id="fieldset-allow-edit"
                    label-cols-xs="8"
                    label-cols-sm="8"
                    label-cols-lg="8"
                    label="Allow Edit"
                    label-for="input-allow-edit"
                    >
                    <b-form-checkbox id="input-allow-edit" v-model="getClassDefinition.isAllowEdit"></b-form-checkbox>
                </b-form-group>
                <b-form-group
                    id="fieldset-allow-delete"
                    label-cols-xs="8"
                    label-cols-sm="8"
                    label-cols-lg="8"
                    label="Allow Delete"
                    label-for="input-allow-delete"
                    >
                    <b-form-checkbox id="input-allow-delete"  v-model="getClassDefinition.isAllowDelete"></b-form-checkbox>
                </b-form-group>
                <b-form-group
                    id="fieldset-generate-dashboard-menu"
                    label-cols-xs="8"
                    label-cols-sm="8"
                    label-cols-lg="8"
                    label="Generate Dashboard Menu"
                    label-for="input-generate-dashboard-menu"
                    >
                    <b-form-checkbox id="input-generate-dashboard-menu"  v-model="getClassDefinition.isGenerateDashboardMenu"></b-form-checkbox>
                </b-form-group>
                <b-form-group
                    id="fieldset-is-printable"
                    label-cols-xs="8"
                    label-cols-sm="8"
                    label-cols-lg="8"
                    label="Is Printable"
                    label-for="input-is-printable"
                    >
                    <b-form-checkbox id="input-is-printable" v-model="getClassDefinition.isPrintable"></b-form-checkbox>
                </b-form-group>
                <b-form-group
                    id="fieldset-has-approval"
                    label-cols-xs="8"
                    label-cols-sm="8"
                    label-cols-lg="8"
                    label="Has Approval"
                    label-for="input-has-approval"
                    >
                    <b-form-checkbox id="input-has-approval"  v-model="getClassDefinition.hasApprovalPage"></b-form-checkbox>
                </b-form-group>
                <b-form-group
                    id="fieldset-has-colon"
                    label-cols-xs="8"
                    label-cols-sm="8"
                    label-cols-lg="8"
                    label="Has Colon (:)"
                    label-for="input-has-colon"
                    >
                    <b-form-checkbox id="input-has-colon" v-model="getClassDefinition.hasColon"></b-form-checkbox>
                </b-form-group>
                <b-form-group
                    id="fieldset-split-layout"
                    label-cols-xs="8"
                    label-cols-sm="8"
                    label-cols-lg="8"
                    label="Split Layout"
                    label-for="input-split-layout"
                    >
                    <b-form-checkbox id="input-split-layout" v-model="getClassDefinition.isSplitLayout"></b-form-checkbox>
                </b-form-group>
            </div>
        </div>
        <div v-if="isShow" id="property-definition-sidebar">
            <div class="pr-3 pl-3 pt-1 pb-1">
                <b-form-group
                    id="fieldset-required"
                    label-cols-xs="8"
                    label-cols-sm="8"
                    label-cols-lg="8"
                    label="Required"
                    label-for="input-required"
                    >
                    <b-form-checkbox 
                        id="input-required"
                        v-model="selectedPropertyDefinition.isRequired"
                        selected="1"
                        not_selected="0"
                        ></b-form-checkbox>
                </b-form-group>
                <b-form-group
                    id="fieldset-show-on-list"
                    label-cols-xs="8"
                    label-cols-sm="8"
                    label-cols-lg="8"
                    label="Show On List"
                    label-for="input-show-on-list"
                    >
                    <b-form-checkbox 
                        id="input-show-on-list"
                        v-model="selectedPropertyDefinition.isShowList"
                        selected="1"
                        not_selected="0"
                        ></b-form-checkbox>
                </b-form-group>
                <b-form-group
                    id="fieldset-check-duplicate"
                    label-cols-xs="8"
                    label-cols-sm="8"
                    label-cols-lg="8"
                    label="Check Duplicate"
                    label-for="input-check-duplicate"
                    >
                    <b-form-checkbox 
                        id="input-check-duplicate"
                        v-model="selectedPropertyDefinition.checkForDuplicate"
                        selected="1"
                        not_selected="0"
                        ></b-form-checkbox>
                </b-form-group>
                <b-form-group
                    id="fieldset-filter"
                    label-cols-xs="8"
                    label-cols-sm="8"
                    label-cols-lg="8"
                    label="Filter"
                    label-for="input-filter"
                    >
                    <b-form-checkbox 
                        id="input-filter"
                        v-model="selectedPropertyDefinition.isFilter"
                        selected="1"
                        not_selected="0"
                        ></b-form-checkbox>
                </b-form-group>
                 <b-form-group
                            id="fieldset-filter-method"
                            label-cols-sm="12"
                            label-cols-lg="12"
                            label=""
                            label-for="input-filter-method"
                            style="margin-top: -20px;"
                            v-if="selectedPropertyDefinition.isFilter"
                            >
                            <b-form-select 
                                v-model="selectedPropertyDefinition.filterMethod" 
                                :options="filterCombo.options" ></b-form-select>
                </b-form-group>
                <b-form-group
                    id="fieldset-is-collection"
                    label-cols-xs="8"
                    label-cols-sm="8"
                    label-cols-lg="8"
                    label="Is Collection"
                    label-for="input-is-collection"
                    >
                    <b-form-checkbox 
                        id="input-is-collection"
                        v-model="selectedPropertyDefinition.isCollection"
                        selected="1"
                        not_selected="0"
                        ></b-form-checkbox>
                </b-form-group>
            </div>
            <hr class="mt-0 mb-1" />
            <div class="pr-3 pl-3 mt-3">
                <strong class="mb-4">Database Setting</strong>
                <b-form-group
                    id="fieldset-check-primary-key"
                    label-cols-xs="8"
                    label-cols-sm="8"
                    label-cols-lg="8"
                    label="Primary Key"
                    label-for="input-check-primary-key"
                    >
                    <b-form-checkbox 
                        id="input-check-primary-key"
                        v-model="selectedPropertyDefinition.isPrimaryKey"
                        selected="1"
                        not_selected="0"
                        >
                    </b-form-checkbox>
                </b-form-group>
                <b-form-group
                    id="fieldset-check-primary-value"
                    label-cols-xs="8"
                    label-cols-sm="8"
                    label-cols-lg="8"
                    label="Primary Value"
                    label-for="input-check-primary-value"
                    >
                    <b-form-checkbox 
                        id="input-check-primary-value"
                        v-model="selectedPropertyDefinition.isPrimaryValue"
                        selected="1"
                        not_selected="0"
                        >
                    </b-form-checkbox>
                </b-form-group>

                <b-form-group
                    id="fieldset-check-is-autonumber"
                    label-cols-xs="8"
                    label-cols-sm="8"
                    label-cols-lg="8"
                    label="Auto Number"
                    label-for="input-check-is-autonumber"
                    >
                    <b-form-checkbox 
                        id="input-check-is-autonumber"
                        v-model="selectedPropertyDefinition.isAutonumber"
                        selected="1"
                        not_selected="0"
                        >
                    </b-form-checkbox>
                </b-form-group>
                
                <b-form-group
                            id="fieldset-field-name"
                            label-cols-sm="12"
                            label-cols-lg="12"
                            label="Field Name"
                            label-for="input-field-name"
                            >
                            <b-form-input id="input-field-name"  v-model="selectedPropertyDefinition.dbFieldName" size="sm"></b-form-input>
                </b-form-group>

                <b-form-group
                            id="fieldset-field-type"
                            label-cols-sm="12"
                            label-cols-lg="12"
                            label="Field Type"
                            label-for="input-field-type"
                            >
                            <b-form-select 
                                v-model="selectedPropertyDefinition.dbFieldTypeId" 
                                :options="getDBFieldTypeCombo" ></b-form-select>
                </b-form-group>

                <b-form-group
                            id="fieldset-field-size"
                            label-cols-sm="12"
                            label-cols-lg="12"
                            label="Field Size"
                            label-for="input-field-size"
                            >
                            <b-form-input id="input-field-size" v-model="selectedPropertyDefinition.dbFieldSize" size="sm"></b-form-input>
                </b-form-group>
            </div>
            <hr class="mt-0 mb-1" />
            <div class="pr-3 pl-3 mt-3">
                <strong class="mb-4">Input Setting</strong>
                <b-form-group
                    id="fieldset-is-hidden"
                    label-cols-xs="8"
                    label-cols-sm="8"
                    label-cols-lg="8"
                    label="Hidden"
                    label-for="input-is-hidden"
                    >
                    <b-form-checkbox 
                        id="input-is-hidden"
                        v-model="selectedPropertyDefinition.isHidden"
                        selected="1"
                        not_selected="0"
                        ></b-form-checkbox>
                </b-form-group>
                <b-form-group
                            id="fieldset-max-value"
                            label-cols-sm="12"
                            label-cols-lg="12"
                            label="Max Value"
                            label-for="input-max-value"
                            >
                            <b-form-input id="input-max-value"  v-model="selectedPropertyDefinition.maxValue" size="sm"></b-form-input>
                </b-form-group>
                <b-form-group
                            id="fieldset-min-value"
                            label-cols-sm="12"
                            label-cols-lg="12"
                            label="Min Value"
                            label-for="input-min-value"
                            >
                            <b-form-input id="input-min-value" v-model="selectedPropertyDefinition.minValue" size="sm"></b-form-input>
                </b-form-group>
                <b-form-group
                            id="fieldset-prepend-text"
                            label-cols-sm="12"
                            label-cols-lg="12"
                            label="Prepend Text"
                            label-for="input-prepend-text"
                            >
                            <b-form-input id="input-prepend-text" v-model="selectedPropertyDefinition.prependText" size="sm"></b-form-input>
                </b-form-group>
                <b-form-group
                            id="fieldset-append-text"
                            label-cols-sm="12"
                            label-cols-lg="12"
                            label="Append Text"
                            label-for="input-append-text"
                            >
                            <b-form-input id="input-append-text"  v-model="selectedPropertyDefinition.appendText" size="sm"></b-form-input>
                </b-form-group>
            </div>
        </div>
    </div>
</template>
<script>
import { dbFieldTypeAPI } from "@/api/DBFieldTypeAPI";
import { defaultFieldType } from '@/store/defaultValue';

export default {
    mixins : [dbFieldTypeAPI],
    methods : {
        async getDBFieldType() {
            this.dbFieldTypeCombo.value = this.$store.state.dbFieldType;
            this.dbFieldTypeCombo.options = this.dbFieldTypeCombo.value;    
            var comboText = (this.selectedPropertyDefinition.propertyType != null ? this.selectedPropertyDefinition.propertyType.name.toLowerCase() : "");
            var dbFieldTypeOptions = [];
            this.dbFieldTypeCombo.value.forEach(item => {
                if (defaultFieldType != undefined) {
                    if (defaultFieldType[comboText] != undefined) {
                        if (defaultFieldType[comboText][item.text] != undefined) {
                            dbFieldTypeOptions.push(item);
                        }
                    }
                }
            });
            this.dbFieldTypeCombo.options = dbFieldTypeOptions;
        },
    },
    data() {
        return {
            statusProp : false,
            dbFieldTypeCombo : {
                selected : '',
                value:[],
                options : []
            },
            filterCombo : {
                selected : '',
                options : [
                    {
                        value : 'contains',
                        text : 'Contains Specification'
                    },
                    {
                        value : 'equal',
                        text : 'Equal Specification'
                    }
                ]
            },
            isLoaded : false
        }
    },
    computed: {
        isShow() {
            return this.$store.state.selectPropertyDefinition;
        },
        selectedPropertyDefinition() {
            return this.$store.state.selectedPropertyDefinition;
        },
        getClassDefinition() {
            return this.$store.state.classDefinition;
        },
        getDBFieldTypeCombo: {
            get: function () {
                return this.$store.state.dbFieldType;
            },
            set: function (value) {
                console.log(value);
            }
        },
        checkData() {
            if (this.isLoaded) {
                this.propertyTypeChanged();
                return true;
            }
            return false;
        }
    },
    mounted : function() {
        //this.getDBFieldType();
    },
    props : ["propertyDefinition", "classDefinition"]
    
}
</script>