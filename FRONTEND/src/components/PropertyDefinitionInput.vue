<template>
        <b-card v-on:click="toggleSidebar" :bg-variant="getVariant" :v-if="checkData">
            <b-button size="sm" class="float-right ml-2 p-0" variant="danger" @click="removeItem(item)">
                <b-icon icon="x" />
            </b-button>
            <b-row>
                <b-col>
                    <b-form-group
                            id="fieldset-name"
                            label-cols-sm="12"
                            label-cols-lg="12"
                            label="Property Name"
                            label-for="input-property-name"
                            >
                            <b-form-input :id="'input-property-name-' + item.rowIndex" v-model="item.propertyName" v-on:change="classNameChanged" size="sm"></b-form-input>
                    </b-form-group>
                    <b-form-group
                            id="fieldset-name"
                            label-cols-sm="12"
                            label-cols-lg="12"
                            label="Property Label"
                            label-for="input-property-label"
                            >
                            <b-form-input :id="'input-property-label-' + item.rowIndex" v-model="item.label" size="sm"></b-form-input>
                    </b-form-group>
                </b-col>
                <b-col>
                    <b-form-group
                            id="fieldset-name"
                            label-cols-sm="12"
                            label-cols-lg="12"
                            label="Property Type"
                            label-for="input-property-type"
                            >
                            <b-form-select :id="'input-property-type-' + item.rowIndex" v-model="item.propertyTypeId" :options="propertyTypeCombo.options" v-on:change="propertyTypeChanged" @blur="setDBFieldTypeCombo" v-on:blur="setDBFieldTypeCombo"></b-form-select>
                    </b-form-group>
                    <b-form-group
                            id="fieldset-name"
                            label-cols-sm="12"
                            label-cols-lg="12"
                            label="Input Type"
                            label-for="input-type"
                            >
                            <b-form-select :id="'input-type-' + item.rowIndex " v-model="item.inputTypeId" :options="inputTypeCombo.options" ></b-form-select>
                    </b-form-group>
                </b-col>
                <b-col>

                    <loading-content-line :column="2" :busy="isClassDefinitionLoading" :width="240" :height="25" orientation="vertical" v-if="showObject">
                        <b-form-group
                                id="fieldset-name"
                                label-cols-sm="12"
                                label-cols-lg="12"
                                label="Object Name"
                                label-for="input-object-name"
                                >
                                
                                <b-input-group>
                                    <b-form-select :id="'input-object-name-' + item.rowIndex " v-model="item.objectDefinitionId" :options="classDefinitionCombo.options" ></b-form-select>
                                    <b-input-group-append>
                                    <b-button v-b-modal.popup-class-definition>+</b-button>
                                    </b-input-group-append>
                                </b-input-group>
                        </b-form-group>
                    </loading-content-line>
                </b-col>
            </b-row>
        </b-card>
</template>
<style scoped>
    .bg-light {
        background-color: #EDFAFF !important;
    }
</style>
<script>
import { mapActions } from 'vuex'

import { classDefinitionAPI } from "@/api/ClassDefinitionAPI";
import { propertyTypeAPI } from "@/api/PropertyTypeAPI";
import { inputTypeAPI } from "@/api/InputTypeAPI";
import { dbFieldTypeAPI } from "@/api/DBFieldTypeAPI";

import { defaultInput, defaultFieldType } from '@/store/defaultValue';
import EventBus from '@/store/eventBus';
import loadingContentLine from "@/components/LoadingContentLine";

export default { 
    mixins: [ propertyTypeAPI, inputTypeAPI, classDefinitionAPI, dbFieldTypeAPI],
    components: {loadingContentLine},
    methods : {
        toggleSidebar : function() {
            var status = true;
            this.$store.dispatch('selectPropertyDefinition', status);

            var selectedPropertyDefinition = this.item;
            this.$store.dispatch('selectedPropertyDefinition', selectedPropertyDefinition);

            this.setDBFieldTypeCombo();
        },
        setDBFieldTypeCombo : function() {
            var dbFiledTypes = this.dbFieldTypeCombo.options;
            this.$store.dispatch('setDBFieldType', dbFiledTypes);
        },
        removeItem : function(item) {
             var status = false;
            this.$store.dispatch('selectPropertyDefinition', status);

            var selectedPropertyDefinition = {};
            this.$store.dispatch('selectedPropertyDefinition', selectedPropertyDefinition);
            
            this.$store.dispatch('setDBFieldType', []);
            this.$store.dispatch('removePropertyDefinition', item);
        },
        async getClassDefinition() {
            var classDefinition = this.$store.state.classDefinition;
            var projectDefinitionId = "";
            if (classDefinition != null) {
                if (classDefinition.projectDefinitionDetail.length > 0) {
                    projectDefinitionId = classDefinition.projectDefinitionDetail[0].parentId;
                    this.isClassDefinitionLoading = true;
                    this.classDefinitionCombo.options = 
                        await this.getClassDefinitionCombo(projectDefinitionId);
                    this.isClassDefinitionLoading = false;
                }
            }
        },
        async getPropertyType() {
            this.propertyTypeCombo.options = await this.getPropertyTypeCombo();
            return true;
        },
        async getInputType() {
            this.inputTypeCombo.value = await this.getInputTypeCombo();
            this.inputTypeCombo.options = this.inputTypeCombo.value;
            this.isLoaded = true;
        },
        async getDBFieldType() {
            this.dbFieldTypeCombo.value = await this.getDBFieldTypeCombo();
        },
        classNameChanged: function() {
            this.item.label = this.convertToLabel(this.item.propertyName);
            this.item.dbFieldName = this.convertToDbField(this.item.propertyName);
        },
        async propertyTypeChanged() {
            var propertyId = this.item.propertyTypeId;
            var comboText = "";
            if (this.propertyTypeCombo.options != null) {
                this.propertyTypeCombo.options.forEach(item => {
                    if (item.value == propertyId) {
                        comboText = item.text.toLowerCase();
                    }
                });
            }
            var inputOptions = [];
            this.inputTypeCombo.value.forEach(item => {
                if (defaultInput != undefined) {
                    if (defaultInput[comboText] != undefined) {
                        if (defaultInput[comboText][item.text] != undefined) {
                            inputOptions.push(item);
                        }
                    }
                }
            });
            this.inputTypeCombo.options = inputOptions;

            var dbFieldTypeComboValue = this.dbFieldTypeCombo.value;
            //comboText = (this.item.propertyType != null ? this.item.propertyType.name.toLowerCase() : "");
            var dbFieldTypeOptions = [];
            dbFieldTypeComboValue.forEach(item => {
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
        convertToLabel(text) {
            var i=0;
            var character='';
            var newText = '';
            while (i <= text.length){
                character = text.charAt(i);
                if (isNaN(character * 1)){
                    if (character == character.toUpperCase()) {
                        if (i > 0) {
                            character = ' ' + character;
                        }
                    }
                }
                newText += character;
                i++;
            }
            return newText;
        },
        convertToDbField(text) {
            var i=0;
            var character='';
            var newText = '';
            while (i <= text.length){
                character = text.charAt(i);
                if (isNaN(character * 1)){
                    if (character == character.toUpperCase()) {
                        if (i > 0) {
                            character = '_' + character.toLowerCase();
                        }
                    }
                }
                newText += character;
                i++;
            }
            return newText.toLowerCase();
        },
    },
    computed : {
        showObject() {
            var selectedText = "";
            this.propertyTypeCombo.options.forEach(element => {
                if (element.value == this.item.propertyTypeId) {
                    selectedText = element.text;
                }
            });

            if (selectedText.toLowerCase() == "object") {
                return true;
            } else {
                return false;
            }
        },
        getVariant() {
            var selectedItem = this.$store.state.selectedPropertyDefinition;
            if (this.item.rowIndex != undefined && selectedItem.rowIndex != undefined) {
                if (this.item.rowIndex == selectedItem.rowIndex) {
                    return "light";
                }
            }
            return "default";
        },
        checkData() {
            if (this.isLoaded > 0) {
                this.propertyTypeChanged();
                return true;
            }
            return false;
        }
    },
     ...mapActions([
      'selectedPropertyDefinition',
      'selectPropertyDefinition',
      'removePropertyDefinition'
    ]),
    data() {
        return {
            classDefinitionCombo : {
                selected : '',
                options : []
            },
            propertyTypeCombo : {
                selected : '',
                options : []
            },
            inputTypeCombo : {
                selected : '',
                value: [],
                options : []
            },
            dbFieldTypeCombo : {
                selected : '',
                value: [],
                options : []
            },
            isLoaded : false,
            isClassDefinitionLoading: false
        }
    },
    mounted() {
        this.getClassDefinition();
        this.getPropertyType();
        this.getInputType();
        this.getDBFieldType();
        EventBus.$on('CLASS_DEFINITION_DATA_ADDED', (payload) => {
            if (payload) {
                this.getClassDefinition();
            }
        });
    },
    props: ['item']
}
</script>